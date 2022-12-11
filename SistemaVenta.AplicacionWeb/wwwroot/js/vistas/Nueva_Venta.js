// Variables globales para representar los productos de venta y el impuesto
let productosParaVenta = [];
let valorImpuesto = 0

// Funcion principal para mostrar la data
$(document).ready(function () {

  // llamado a los tipos de documento de venta como factura, boleta
  fetch("/Venta/ListaTipoDocumentoVenta")
    .then(response => {
      return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
      if (responseJson.length > 0) {
        responseJson.forEach((item) => {
          $("#cboTipoDocumentoVenta").append(
            $("<option>").val(item.idTipoDocumentoVenta).text(item.descripcion)
          )
        })
      }
    })

  // llamado para obtener los valores del negocio como simbolo de moneda, porcentaje impuesto
  fetch("/Negocio/Obtener")
    .then(response => {
      return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
      if (responseJson.estado) {
        const objeto = responseJson.objeto;
        console.log(objeto)

        $("#inputGroupSubTotal").text(`Sub total - ${objeto.simboloMoneda}`)
        $("#inputGroupIGV").text(`IVA(${objeto.porcentajeImpuesto}%) - ${objeto.simboloMoneda}`)
        $("#inputGroupTotal").text(`Total - ${objeto.simboloMoneda}`)

        valorImpuesto = parseFloat(objeto.porcentajeImpuesto)

      }
    })

  // Lógica para el buscador de productos
  $("#cboBuscarProducto").select2({
    ajax: {
      url: "/Venta/ObtenerProductos",
      dataType: 'json',
      contentType: "application/json; charset=utf-8",
      delay: 250,
      data: function (params) {
        return {
          busqueda: params.term
        };
      },
      processResults: function (data) {

        return {
          results: data.map((item) => (
            {
              id: item.idProducto,
              text: item.descripcion,
              marca: item.marca,
              categoria: item.nombreCategoria,
              urlImagen: item.urlImagen,
              precio: parseFloat(item.precio)
            }
          ))
        };
      },
    },
    language: "es",
    placeholder: 'Buscar Producto...',
    minimumInputLength: 1,
    templateResult: formatoResultados
  });

})


// Template para el buscador de resultados
function formatoResultados(data) {
  //esto es por defecto, ya que muestra el "buscando..."
  if (data.loading)
    return data.text

  let contenedor = $(
    `<table width="100%"
      <tr>
        <td style="width:60px">
          <img style="height:60px;width:60px;margin-right:10px" src="${data.urlImagen}"/>
        </td>
        <td>
          <p style="font-weight:bolder;margin:2px">${data.marca}</p>
          <p style="margin:2px">${data.text}</p>
        </td>
      </tr>
     </table>`
  )
  return contenedor;
}

// Funcion para hacer que el cursor se ponga en el buscador cuando lo usamos
$(document).on("select2:open", function () {
  document.querySelector(".select2-search__field").focus()
})


// Lógica para poder hacer una venta
$("#cboBuscarProducto").on("select2:select", function (e) {
  const data = e.params.data;

  let productoEncontrado = productosParaVenta.filter(p => p.idProducto === data.id)
  if (productoEncontrado.length > 0) {
    $("#cboBuscarProducto").val("").trigger("change")
    toastr.warning("", "El producto ya fue agregado")
    return false
  }

  swal({
    title: data.marca,
    text: data.text,
    imageUrl: data.urlImagen,
    type: "input",
    showCancelButton: true,
    closeOnConfirm: false,
    inputPlaceholder: "Ingrese Cantidad"
  },
    function (valor) {

      if (valor === false) return false;

      if (valor === "") {
        toastr.warning("", "Necesita ingresar la cantidad")
        return false
      }

      if (isNaN(parseInt(valor))) {
        toastr.warning("", "Debe ingresar un valor númerico")
        return false
      }

      let producto = {
        idProducto: data.id,
        marcaProducto: data.marca,
        descripcionProducto: data.text,
        categoriaProducto: data.categoria,
        cantidad: parseInt(valor),
        precio: data.precio.toString(),
        total: (parseFloat(valor) * data.precio).toString()
      }

      productosParaVenta.push(producto)

      mostrarPreciosProductos()
      $("#cboBuscarProducto").val("").trigger("change")
      swal.close()

    }
  )

})

// Logica para mostrar los datos del producto que se va a vender

function mostrarPreciosProductos() {
  let total = 0
  let iva = 0
  let subtotal = 0
  let porcentaje = valorImpuesto / 100;

  $("#tbProducto tbody").html("")

  productosParaVenta.forEach((item) => {

    total += parseFloat(item.total)

    $("#tbProducto tbody").append(
      $("<tr>").append(
        $("<td>").append(
          $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
            $("<i>").addClass("fas fa-trash-alt")
          ).data("idProducto", item.idProducto)
        ),
        $("<td>").text(item.descripcionProducto),
        $("<td>").text(item.cantidad),
        $("<td>").text(`$${item.precio}`),
        $("<td>").text(`$${item.total}`)
      )
    )
  })

  subtotal = total / (1 + porcentaje);
  iva = total - subtotal;

  $("#txtSubTotal").val(subtotal.toFixed(2))
  $("#txtIGV").val(iva.toFixed(2))
  $("#txtTotal").val(total.toFixed(2))

}

// Botones para eliminar

$(document).on("click", "button.btn-eliminar", function () {

  const _idProducto = $(this).data("idProducto")
  productosParaVenta = productosParaVenta.filter(p => p.idProducto !== _idProducto);

  mostrarPreciosProductos();

})


// Lógica para terminar la venta haciendo click en el boton terminar venta
$("#btnTerminarVenta").click(function () {

  if (productosParaVenta.length < 1) {
    toastr.warning("", "Debe ingresar productos")
    return
  }

  const vmDetalleVenta = productosParaVenta;

  const venta = {
    idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
    documentoCliente: $("#txtDocumentoCliente").val(),
    nombreCliente: $("#txtNombreCliente").val(),
    subTotal: $("#txtSubTotal").val(),
    impuestoTotal: $("#txtIGV").val(),
    total: $("#txtTotal").val(),
    detalleVenta: vmDetalleVenta
  }
  console.log(JSON.stringify(venta))
  $("#btnTerminarVenta").LoadingOverlay("show")

  fetch("/Venta/RegistrarVenta", {
    method: "POST",
    headers: { "Content-Type": "application/json; charset=utf-8" },
    body: JSON.stringify(venta)
  })
    .then(response => {
      $("#btnTerminarVenta").LoadingOverlay("hide")
      return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
      if (responseJson.estado) {
        productosParaVenta = []
        mostrarPreciosProductos()

        $("#txtDocumentoCliente").val("")
        $("#txtNombreCliente").val("")
        $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())

        swal("Registrado!", `Número venta: ${responseJson.objeto.numeroVenta}`, "success")
      } else {
        swal("Lo Sentimos", "No se pudo registrar la venta", "error")
      }
    })

})

