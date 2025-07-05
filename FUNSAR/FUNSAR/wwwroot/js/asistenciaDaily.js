var dataTable;
var dropdownOptions = [];

$(document).ready(function () {
    $.when(cargarDropdownData()).done(function () {
        cargarDatatable();
    });
});

function cargarDropdownData() {
    $.ajax({
        url: '/admin/Asistencias/GetDropdownData',
        type: 'GET',
        success: function (data) {
            dropdownOptions = data;
        }
    });
}

function cargarDatatable() {
    dataTable = $("#tblAsistenciaDia").DataTable({
        "ajax": {
            "url": "/admin/Asistencias/GetAllDaily",
            "type": "GET",
            "datatype": "json"
        },
        "paging": false,
        "columns": [
            {
                "data": "id", "width": "5%", "render": function (data, type, row) {
                    return '<tr data-id="' + row.id + '"><td>' + data + '</td></tr>';
                }
            },
            { "data": "nombre", "width": "10%" },
            { "data": "apellido", "width": "10%" },
            { "data": "tipoDocumento.tDocumento", "width": "5%" },
            { "data": "documento", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    var select = '<select class="form-control">';
                    $.each(dropdownOptions, function (index, value) {
                        select += '<option value="' + value + '">' + value + '</option>';
                    });
                    select += '</select>';
                    return select;
                }, "width": "10%"
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "width": "100%"
    });
}

$('#sendDataBtn').click(function () {
    var tableData = [];
    var dateIn = document.getElementById('lblFecha').textContent;
    dateIn = dateIn.substring(21,31);

    if (dateIn != '01/01/0001') {

        var parts = dateIn.split('/');
        var formattedDate = parts[2] + '-' + parts[1] + '-' + parts[0]; // yyyy-MM-dd

        $('#tblAsistenciaDia tbody tr').each(function () {
            var row = $(this);
            var itemId = row.find('td:eq(0)').text();
            var selectedValue = row.find('select').val();

            tableData.push({
                ItemId: itemId,
                SelectedValue: selectedValue
            });
        });

        $.ajax({
            url: '/admin/Asistencias/SaveTableData?dateIn=' + formattedDate,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(
                tableData
            ),
            success: function (response) {
                if (response.success && response.validate) {
                    toastr.success('Datos guardados correctamente');
                } else if (response.success && !response.validate) {
                    swal.fire({
                        title: "Información",
                        text: "Algunos voluntarios ya contaban con el limite de asistencia por dia.",
                        type: "info",
                        showCancelButton: false,
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "Entendido",
                        closeOnConfirm: true
                    });
                } else if (!response.success && response.validate) {
                    toastr.error('Error al guardar los datos');
                }
                else if (!response.success && !response.validate) {
                    toastr.error('Error al guardar los datos');
                }
            }
        });
    } else {
        toastr.error('Los datos ingresados no son validos');
    }

    
});

