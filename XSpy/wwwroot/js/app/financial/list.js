require(['/js/common.js'],
    function (common) {
require(['jquery', 'select2', 'tables', 'utils', 'toastr'],
    function($, select2, tables, utils, toastr) {

        $(document).ready(function () {
            tables.setExportOptions(['0', '1']);



            $('.select2').select2({
                allowClear: true
            });


            window.dataTable = $('#dataTable').DataTable({
                "ajax": {
                    "contentType": 'application/json',
                    "data": function(d) {
                        d.filter = utils.getFormData($("#searchForm"));
                        window.lastSearch = d;
                        return JSON.stringify(d);
                    },
                    "url": '/api/financial/orders/list',
                    "type": "POST"
                },
                columns: [
                    { data: 'userData.email' },
                    { data: 'userData.name' },
                    {
                        data: 'paymentMethod',
                        'mRender': utils.paymentTypeRenderer
                    },
                    {
                        data: 'paymentStatus',
                        'mRender': utils.transactionStatusRenderer
                    },
                    {
                        data: 'value',
                        'mRender': utils.floatRender
                    },
                    {
                        data: 'createdAt',
                        'mRender': utils.dateRenderer
                    },
                    {
                        data: 'null',
                        'mRender': function(data, type, row, meta) {
                            var buttons =
                                `<a data-toggle="tooltip" class="text-warning table-icon" title="Acessar recibo" style='margin:1px;' href="details/${
                                    row.id
                                    }">
                                    <i class="fa fa-file-text-o"> </i>
                                    </a>`;

                            if (row.paymentMethod == "Deposit" && row.paymentStatus == "Pending") {

                             
                                if (window.canAccept) {
                                    buttons +=
                                        `<a data-toggle="tooltip" title="Anexar comprovante" class="text-info table-icon action-attach"
                                    href="#"><i class="fa fa-plus-square" aria-hidden="true"></i></a>`;

                                    buttons +=
                                        `<a data-toggle="tooltip" class="text-navy table-icon" onclick="acceptPayment('${
                                        row.id}')" title="Aprovar pagamento">
                                    <i class="fa fa-check-square" aria-hidden="true"></i></a>`;
                                }

                                if (row.extraData) {
                                    buttons +=
                                        `<a data-toggle="tooltip" class="text-danger table-icon" style='margin:1px;' href="${
                                        JSON.parse(row.extraData).url_slip_pdf}" title="Acessar boleto" target="_blank">
                                    <i class="fa fa-external-link-square" aria-hidden="true"></i></a>`;
                                }

                            }

                            return buttons;

                        }
                    }
                ]

            });

    
            dataTable.on('responsive-display',
                function (e, datatable, row, showHide, update) {
                    if (showHide) {
                        $('[data-toggle="tooltip"]').tooltip({
                            container: "#dataTable_wrapper"
                        });

                        $('.action-attach').click(function () {
                            $("#attachDocumentModal").modal();
                        });
                    }
                });
        });
    });
    });