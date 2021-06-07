
function BindDataTable(tableIdWithoutSharp, isSortable = true, hasInitialOrder = true) {
    $(document).ready(function () {
        $("#" + tableIdWithoutSharp).DataTable({
            "bSort": isSortable,
            "order": hasInitialOrder ? [[0, 'asc']] : []
        });
    });
}
