
//Lógica para mostrar los elementos

const VISTA_BUSQUEDA = {
  //Funcion para mostrar las cajas de texto
  busquedaFecha: () => {
    $("#txtFechaInicio").val("")
    $("#txtFechaFin").val("")
    $("#txtNumeroVenta").val("")

    $(".busqueda-fecha").show()
    $(".busqueda-venta").hide()
  },
  busquedaVenta: () => {
    $("#txtFechaInicio").val("")
    $("#txtFechaFin").val("")
    $("#txtNumeroVenta").val("")

    $(".busqueda-fecha").hide()
    $(".busqueda-venta").show()

  }
}

// Lógica para cuando se eliga el buscar por historial de venta aparezcan los inputs y aparezca el datepicker de jquery
$(document).ready(function () {
  VISTA_BUSQUEDA["busquedaFecha"]()

  $.datepicker.setDefaults($.datepicker.regional["es"])

  $("#txtFechaInicio").datepicker({dateFormat: "dd/mm/yy"})
  $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

})
// Lógica para que segun el combobox que eliga ya sea numero venta o fecha aparezcan o desaparezcan
$("#cboBuscarPor").change(function () {
  if ($("#cboBuscarPor").val() === "fecha") {
    VISTA_BUSQUEDA["busquedaFecha"]()
  } else {
    VISTA_BUSQUEDA["busquedaVenta"]()
  }
})

// Toda la lógica para el boton de buscar 

$("#btnBuscar").click(function () {

  // Se valida si se busca por fecha o por numero de venta

  if ($("#cboBuscarPor").val() == "fecha") {

    if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
      toastr.warning("", "Debe ingresar una fecha de inicio y una fecha fin")
      return
    } 

  } else {

    if ($("#txtNumeroVenta").val().trim() == "") {
      toastr.warning("", "Debe ingresar un número de venta")
      return
    }
  }

  // se inicializan variables para despues pasarla como argumento al fetch

  let numeroVenta = $("#txtNumeroVenta").val()
  let fechaInicio = $("#txtFechaInicio").val()
  let fechaFin = $("#txtFechaFin").val()

  $(".card-body").find("div.row").LoadingOverlay("show")

  fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
    .then(response => {
      $(".card-body").find("div.row").LoadingOverlay("hide")
      return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {

      $("#tbventa tbody").html("");

      // Creacion de un pequeño template para mostrar la data
      if (responseJson.length > 0) {
        responseJson.forEach((venta) => {

          $("#tbventa tbody").append(
            $("<tr>").append(
              $("<td>").text(venta.fechaRegistro),
              $("<td>").text(venta.numeroVenta),
              $("<td>").text(venta.tipoDocumentoVenta),
              $("<td>").text(venta.documentoCliente),
              $("<td>").text(venta.nombreCliente),
              $("<td>").text(venta.total),
              $("<td>").append(
                $("<button>").addClass("btn btn-info btn-sm").append(
                  $("<i>").addClass("fas fa-eye")
                ).data("venta", venta)
              )
            )
          )
        })
      }

    })

})


// Funcionalidad del boton que se crea dinamicamente para que aparezca el modal y muestre la data
$("#tbventa tbody").on("click", ".btn-info", function () {


  let data = $(this).data("venta")
  console.log(data)
  $("#txtFechaRegistro").val(data.fechaRegistro)
  $("#txtNumVenta").val(data.numeroVenta)
  $("#txtUsuarioRegistro").val(data.usuario)
  $("#txtTipoDocumento").val(data.tipoDocumentoVenta)
  $("#txtDocumentoCliente").val(data.documentoCliente)
  $("#txtNombreCliente").val(data.nombreCliente)
  $("#txtSubTotal").val(data.subTotal)
  $("#txtIGV").val(data.impuestoTotal)
  $("#txtTotal").val(data.total)

  $("#tbProductos tbody").html("");

  data.detalleVenta.forEach((item) => {
    
    $("#tbProductos tbody").append(
      $("<tr>").append(
        $("<td>").text(item.descripcionProducto),
        $("<td>").text(item.cantidad),
        $("<td>").text(item.precio),
        $("<td>").text(item.total)
      )
    )

  })

  $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?numeroVenta=${data.numeroVenta}`)


  $("#modalData").modal("show");

})