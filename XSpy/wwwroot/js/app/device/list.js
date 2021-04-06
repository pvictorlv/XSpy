require(['./common'],
    function(common) {
        require(['jquery', 'tables', 'utils'],
            function($, tables, utils) {
                $(document).ready(function() {

                    window.dataTable = $('#dataTable').DataTable({
                        "ajax": {
                            "contentType": 'application/json',
                            "data": function (d) {
                                d.filter = utils.getFormData($("#searchForm"));
                                window.lastSearch = d;
                                return JSON.stringify(d);
                            },
                            "url": '/api/device/list',
                            "type": "POST"
                        },
                        responsive: true,
                        "searching": false,
                        "orderMulti": true,
                        statusCode: {
                            403: function () {
                                window.location.reload();
                            }
                        },
                        columns: [
                            { data: 'username' },
                            {
                                data: 'score'
                            },
                            {
                                data: 'startedAt',
                                render: utils.utcDateRenderer
                            },
                            {
                                data: 'finishedAt',
                                render: utils.utcDateRenderer
                            },
                            {
                                data: null,

                                "mRender": function (data, type, full) {
                                    var buttons = '';

                                    if (full.finishedAt)
                                        buttons +=
                                            `<a class="btn btn-info btn-sm btn-margin" href="result/${data.id
                                            }">Resultado</a>`;
                                    else
                                        buttons +=
                                            `<a class="btn btn-primary btn-sm btn-margin" href="start">Acessar simulado</a>`;

                                    return buttons;
                                }
                            }
                        ]

                    });
                });
            }
        );
    }
);