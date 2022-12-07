// Esto hace referencia a las propiedades del View Model Categoria
const MODELO_BASE = {
  idCategoria: 0,
  descripcion: "",
  esActivo: 1,
}

let tablaData;
// FUNCION PRINCIPAL PARA MOSTRAR LA DATA
$(document).ready(function () {

  // MOSTRAR DATATABLE CON LOS DATOS DE LAS CATEGORIAS
  tablaData = $('#tbdata').DataTable({
    responsive: true,
    "ajax": {
      "url": '/Categoria/Lista',
      "type": "GET",
      "datatype": "json"
    },
    "columns": [
      { "data": "idCategoria", "visible": false, "searchable": false },
      { "data": "descripcion" },
      {
        "data": "esActivo", render: function (data) {
          if (data === 1)
            return '<span class="badge badge-info">Activo</span>'
          else
            return '<span class="badge badge-danger">No Activo</span>'
        }
      },
      {
        "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
          '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
        "orderable": false,
        "searchable": false,
        "width": "80px"
      }
    ],
    order: [[0, "desc"]],
    dom: "Bfrtip",
    buttons: [
      {
        text: 'Exportar Excel',
        extend: 'excelHtml5',
        title: '',
        filename: 'Reporte Categorias',
        exportOptions: {
          columns: [1, 2] //Columnas que se exportaran siguiendo el orden de "columns"
        }
      }, 'pageLength'
    ],
    language: {
      url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
    },
  });
})

// MOSTRAR MODAL

function mostrarModal(modelo = MODELO_BASE) {
  $("#txtId").val(modelo.idCategoria)
  $("#txtDescripcion").val(modelo.descripcion)
  $("#cboEstado").val(modelo.esActivo)

  $("#modalData").modal("show")
}

// Boton para mostrar el modal
$("#btnNuevo").click(function () {
  mostrarModal()
})

// Crear una nueva Categoria
$("#btnGuardar").click(function () {

  if ($("#txtDescripcion").val().trim() == "") {
    toastr.warning("", "Debe completar el campo : descripcion")
    $("#txtDescripcion").focus()
    return
  }

  const modelo = structuredClone(MODELO_BASE);
  modelo["idCategoria"] = parseInt($("#txtId").val())
  modelo["descripcion"] = $("#txtDescripcion").val()
  modelo["esActivo"] = $("#cboEstado").val()

  $("#modalData").find("div.modal-content").LoadingOverlay("show")

  if (modelo.idCategoria == 0) {
    fetch("/Categoria/Crear", {
      method: "POST",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify(modelo)
    })
      .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide")
        return response.ok ? response.json() : Promise.reject(response);
      })
      .then(responseJson => {
        if (responseJson.estado) {
          tablaData.row.add(responseJson.objeto).draw(false)
          $("#modalData").modal("hide")
          swal("Listo!", "La categoria fue creada con éxito", "success")
        } else {
          swal("Lo sentimos", responseJson.mensaje, "error")
        }
      })
  } else {
    fetch("/Categoria/Editar", {
      method: "PUT",
      headers: { "Content-Type": "application/json; charset=utf-8" },
      body: JSON.stringify(modelo)
    })
      .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide")
        return response.ok ? response.json() : Promise.reject(response);
      })
      .then(responseJson => {
        if (responseJson.estado) {
          tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false)
          filaSeleccionada = null
          $("#modalData").modal("hide")
          swal("Listo!", "La categoria fue modificada con éxito", "success")
        } else {
          swal("Lo sentimos", responseJson.mensaje, "error")
        }
      })
  }

})

// Editar una Categoria
let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
  if ($(this).closest("tr").hasClass("child")) {
    filaSeleccionada = $(this).closest("tr").prev()
  } else {
    filaSeleccionada = $(this).closest("tr")
  }

  const data = tablaData.row(filaSeleccionada).data()
  //console.log(data)
  mostrarModal(data);

})

// Eliminar una Categoria

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
  let fila
  if ($(this).closest("tr").hasClass("child")) {
    fila = $(this).closest("tr").prev()
  } else {
    fila = $(this).closest("tr")
  }

  const data = tablaData.row(fila).data()
  //console.log(data)

  swal({
    title: "¿Está seguro?",
    text: `Eliminar a la categoria "${data.descripcion}"`,
    type: "warning",
    showCancelButton: true,
    confirmButtonClass: "btn-danger",
    confirmButtonText: "Si, eliminar",
    cancelButtonText: "No, volver",
    closeOnConfirm: false,
    closeOnCancel: true
  },
    function (respuesta) {
      if (respuesta) {
        $(".showSweetAlert").LoadingOverlay("show")

        fetch(`/Categoria/Eliminar?idCategoria=${data.idCategoria}`, {
          method: "DELETE"
        })

          .then(response => {
            $(".showSweetAlert").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);

          })
          .then(responseJson => {
            if (responseJson.estado) {

              tablaData.row(fila).remove().draw()

              swal("Listo!", "La categoria fue eliminada con éxito", "success")
            } else {
              swal("Lo sentimos", responseJson.mensaje, "error")
            }
          })
      }
    }
  )

})
