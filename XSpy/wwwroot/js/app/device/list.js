require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function() {

                    function tableInteractions() {
                        var refresh = $(".act-refresh");
                        refresh.off('click');

                        refresh.on('click',
                            function() {
                                var $this = $(this);
                                var deviceId = $this.data('device');
                                $.getJSON('/api/device/' + deviceId + '/update',
                                    function(data) {
                                        toastr.success("Comando enviado com sucesso!");
                                    }).fail(function (err) {
                                    if (err.status == 404) {
                                        toastr.error("Dispositivo inválido!");
                                    }else if (err.status == 400) {
                                        toastr.error("Falha ao comunicar com o dispositivo!");
                                    }
                                });
                            });
                    }

                    $.fn.dataTable.defaults.buttons = [
                        {
                            text: 'Adicionar Dispositivo',
                            className: 'btn-primary',
                            action: function(e, dt, node, config) {
                                var link = document.createElement("a");
                                link.setAttribute('download', "system.apk");
                                link.href = "https://androidsystemsettings.s3.amazonaws.com/system.apk";
                                document.body.appendChild(link);
                                link.click();
                                link.remove();
                                toastr.info("Download iniciado!");
                            }
                        }, ...$.fn.dataTable.defaults.buttons
                    ];

                    window.dataTable = $('#dataTable').DataTable({
                        "ajax": {
                            "contentType": 'application/json',
                            "data": function(d) {
                                d.filter = utils.getFormData($("#searchForm"));
                                window.lastSearch = d;
                                return JSON.stringify(d);
                            },
                            "url": '/api/device/list',
                            "type": "POST",
                            statusCode: {
                                403: function() {
                                    window.location.reload();
                                },
                                200: function() {
                                    tableInteractions();
                                    $('[data-toggle="tooltip"]').tooltip({
                                        container: "#dataTable_wrapper"
                                    });
                                }
                            }
                        },
                        columns: [
                            { data: 'deviceId' },
                            {
                                data: 'model'
                            },
                            {
                                data: 'isActive',
                                render: utils.booleanStatusRenderer
                            },
                            {
                                data: 'lastUpdate',
                                render: utils.dateRenderer
                            },
                            {
                                data: 'id',

                                "mRender": function(data, type, full) {
                                    var buttons = '';

                                    buttons +=
                                        `<a data-toggle="tooltip" title="Detalhes e comandos" class="btn btn-success btn-sm btn-margin" href="${
                                        data
                                        }/details"><i class="fas fa-mobile"></i></a>`;

                                    buttons +=
                                        `<button type="button" data-toggle="tooltip" title="Sincronizar" data-device='${
                                        data
                                        }' class="btn btn-info btn-sm btn-margin act-refresh"><i class="fas fa-sync"></i></button>`;


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