$(document).ready(function () {
    $('#colegioInput').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Cliente/Home/Colegio',
                dataType: 'json',
                data: {
                    term: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.nombre,
                            value: item.id
                        };
                    }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            $('#colegioInput').val(ui.item.label);
            $('#colegioId').val(ui.item.value);
            return false;
        }
    });

    $('#continuarBtn').click(function (e) {
        e.preventDefault();

        // Obtener el valor del colegio seleccionado
        var colegioId = $('#colegioId').val();

        // Validar si el colegioId tiene un valor
        if (colegioId === '') {
            swal.fire({
                title: "Error",
                text: "Por favor, selecciona un colegio válido para continuar",
                type: "error"
            });
        } else {
            // Realizar una comprobación adicional si el colegioId existe en la base de datos
            $.ajax({
                url: '/Cliente/Home/ValidarColegio',
                type: 'POST',
                dataType: 'json',
                data: { id: colegioId },
                success: function (data) {
                    if (data.id) {
                        // Si existe, enviar el formulario
                        $('#miFormulario').submit();
                    } else {
                        // Si no existe, mostrar un mensaje de error
                        swal.fire({
                            title: "Error",
                            text: "El colegio ingresado no es valido, solicita a tu gestor realizar la creación.",
                            type: "error"
                        });
                    }
                },
                error: function () {
                    // Manejar errores si falla la petición AJAX
                    swal.fire({
                        title: "Error",
                        text: "Hubo un problema al verificar el colegio. Por favor, inténtalo nuevamente.",
                        type: "error"
                    });
                }
            });
        }
    });
});
