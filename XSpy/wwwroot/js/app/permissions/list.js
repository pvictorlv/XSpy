require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables'],
            function ($, tables) {

                $(document).ready(function () {

                    tables.setExportOptions(['0', '1']);

                    if (window.canCreate == 'True') {
                        $.fn.dataTable.defaults.buttons = [
                            {
                                text: 'Adicionar cargo',
                                className: 'btn-success',
                                action: function (e, dt, node, config) {
                                    window.location.href = '/permissions/create';
                                }
                            }, ...$.fn.dataTable.defaults.buttons
                        ];
                    }

                    window.dataTable = $('#dataTable').DataTable({
                        "ajax": {
                            "contentType": 'application/json',
                            "data": function(d) {
                                window.lastSearch = d;

                                return JSON.stringify(d);
                            },
                            "url": '/api/permissions/list',
                            "type": "POST"
                        },
                        drawCallback: function () {
                            tables.initTooltip();
                        },
                        columns: [
                            { data: 'id' },
                            { data: 'name' },
                            {
                                data: null,
                                className: "center",

                                "mRender": function(data, type, full) {
                                    var buttons =
                                        `<a href="edit/${data.id}" type="button" data-toggle="tooltip" title="Editar"
                                            class="btn btn-warning btn-sm btn-margin action-edit text-white"><i class="fas fa-pencil-alt"></i></button>`;

                                    return buttons;
                                }
                            }
                        ]

                    });
                });

            });
    });