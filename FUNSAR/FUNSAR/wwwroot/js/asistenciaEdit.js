var dataTable;
var dropdownOptions = [];
var idVoluntario = $('#dataContainer').data('id');

$(document).ready(function () {
    cargarDropdownData();
    cargarDatatable();
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
    dataTable = $("#tblAsistenciaEdit").DataTable({
        "ajax": {
            "url": "/admin/Asistencias/GetEdit?id=" + idVoluntario,
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
            {
                "data": "fecha",
                "render": function (data, type, row) {
                    var date = new Date(data);
                    var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                    return date.toLocaleDateString('es-ES', options);
                },
                "width": "20%"
            },
            {
                "data": "estadoAsistenciaId",
                "render": function (data, type, row) {
                    var estadoMap = {
                        1: "Asistio",
                        2: "Retardo",
                        3: "Falla",
                        4: "Salida"
                    };
                    var estado = estadoMap[row.estadoAsistenciaId];
                    var select = `<select class="form-control select-${estado.toLowerCase()}">`;
                    $.each(dropdownOptions, function (index, value) {
                        var selected = (value === estado) ? 'selected' : '';
                        select += `<option value="${value}" ${selected}>${value}</option>`;
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

    $('#tblAsistenciaEdit tbody tr').each(function () {
        var row = $(this);
        var itemId = row.find('td:eq(0)').text();
        var selectedValue = row.find('select').val();

        tableData.push({
            ItemId: itemId,
            SelectedValue: selectedValue
        });
    });

    $.ajax({
        url: '/admin/Asistencias/EditTableData',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(tableData),
        success: function (response) {
            if (response.success) {
                toastr.success('Datos guardados correctamente');
            } else {
                toastr.error('Error al guardar los datos');
            }
        }
    });
});

