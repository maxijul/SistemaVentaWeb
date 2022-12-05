
// OBTENER
$(document).ready(function () {

  $(".card-body").LoadingOverlay("show")

  fetch("/Negocio/Obtener")
    .then(response => {
      $(".card-body").LoadingOverlay("hide")
      return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {

      console.log(responseJson)

      if (responseJson.estado) {
        const negocio = responseJson.objeto

        $("#txtNumeroDocumento").val(negocio.numeroDocumento)
        $("#txtRazonSocial").val(negocio.nombre)
        $("#txtCorreo").val(negocio.correo)
        $("#txtDireccion").val(negocio.direccion)
        $("#txTelefono").val(negocio.telefono)
        $("#txtImpuesto").val(negocio.porcentajeImpuesto)
        $("#txtSimboloMoneda").val(negocio.simboloMoneda)
        $("#imgLogo").attr("src", negocio.urlLogo)
      } else {
        swal("Lo sentimos", responseJson.mensaje, "error")
      }
    })
})


// GUARDAR CAMBIOS
$("#btnGuardarCambios").click(function () {

  const inputs = $("input.input-validar").serializeArray();
  const inputs_sin_valor = inputs.filter(item => item.value.trim() == "");

  if (inputs_sin_valor.length > 0) {
    const mensaje = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
    toastr.warning("", mensaje)
    $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
    return
  }

  const modelo = {
    numeroDocumento: $("#txtNumeroDocumento").val(),
    nombre: $("#txtCorreo").val(),
    correo: $("#txtCorreo").val(),
    direccion: $("#txtDireccion").val(),
    telefono: $("#txTelefono").val(),
    porcentajeImpuesto: $("#txtImpuesto").val(),
    simboloMoneda: $("#txtSimboloMoneda").val()

  }

  const inputLogo = document.getElementById("txtLogo")
  const formData = new FormData()

  formData.append("logo", inputLogo.files[0])
  formData.append("modelo", JSON.stringify(modelo))

  swal({
    title: "¿Está seguro?",
    text: `Desea editar el negocio`,
    type: "warning",
    showCancelButton: true,
    confirmButtonClass: "btn-danger",
    confirmButtonText: "Si, editar",
    cancelButtonText: "No, volver",
    closeOnConfirm: false,
    closeOnCancel: true
  }, function (respuesta) {
    if (respuesta) {
      $(".showSweetAlert").LoadingOverlay("show")
      fetch("/Negocio/GuardarCambios", {
        method: "PUT",
        body: formData
      })
        .then(response => {
          $(".showSweetAlert").LoadingOverlay("hide")
          $(".card-body").LoadingOverlay("hide")
          return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
          if (responseJson.estado) {
            const negocio = responseJson.objeto
            $("#imgLogo").attr("src", negocio.urlLogo)
            swal("Listo!", "El negocio fue modificado con éxito", "success")
          } else {
            swal("Lo sentimos", responseJson.mensaje, "error")
          }
        })
      }
    }
  )
})


