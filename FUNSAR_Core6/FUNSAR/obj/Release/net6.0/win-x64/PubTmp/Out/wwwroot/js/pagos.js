var dataTable;

$(document).ready(function () {
    cargarDatatable();
});


function cargarDatatable() {
    dataTable = $("#tblPagos").DataTable({
        "ajax": {
            "url": "/admin/pagos/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "documentoP", "width": "5%" },
            { "data": "documentoR", "width": "5%" },
            { "data": "correoR", "width": "20%" },
            { "data": "servicioId", "width":"20%"},
            { "data": "fechapago", "width": "15%" },
            { "data": "estado", "width": "5%" },
            { "data": "secuencia", "width": "5%" }
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