let tablaData;

$(document).ready(function () {

  $.datepicker.setDefaults($.datepicker.regional["es"])

  $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
  $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

  
  // FUNCION PRINCIPAL PARA MOSTRAR LA DATA
  $(document).ready(function () {

    // MOSTRAR DATATABLE CON LOS DATOS DE LAS CATEGORIAS
    tablaData = $('#tbdata').DataTable({
      responsive: true,
      "ajax": {
        "url": '/Reporte/ReporteVenta?fechaInicio=01/01/1991&fechaFin=01/01/1991',
        "type": "GET",
        "datatype": "json"
      },
      "columns": [
        { "data": "fechaRegistro" },
        { "data": "numeroVenta" },
        { "data": "tipoDocumento" },
        { "data": "documentoCliente" },
        { "data": "nombreCliente" },
        { "data": "subTotalVenta" },
        { "data": "impuestoTotalVenta" },
        { "data": "totalVenta" },
        { "data": "producto" },
        { "data": "cantidad" },
        { "data": "precio" },
        { "data": "total" },
      ],
      order: [[0, "desc"]],
      dom: "Bfrtip",
      buttons: [
        {
          text: 'Exportar Excel',
          extend: 'excelHtml5',
          title: '',
          filename: 'Reporte Ventas',
          
        }, 'pageLength'
      ],
      language: {
        url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
      },
    });
  })


})

$("#btnBuscar").click(function () {

  if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
    toastr.warning("", "Debe ingresar una fecha de inicio y una fecha fin")
    return
  }

  let fechaInicio = $("#txtFechaInicio").val().trim()
  let fechaFin = $("#txtFechaFin").val().trim()

  let nuevaUrl = `/Reporte/ReporteVenta?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`

  tablaData.ajax.url(nuevaUrl).load()

})