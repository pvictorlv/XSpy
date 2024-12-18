﻿require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function() {
                    tables.setExportOptions(['0', '1', '2', '3', '4']);

                    $('#quickactions>[data-toggle="tooltip"]').tooltip({
                        container: "#quickactions"
                    });

                    $('[data-toggle="tooltip"]').tooltip();


                    $('#dataTable').DataTable({
                        serverSide: false
                    });
                    var fileTables = $('#dataTable_2').DataTable({
                        serverSide: false
                    });

                    $('.act-download').on('click',
                        function () {
                            var $this = $(this);
                            downloadFile($this.data('path'));
                        });
                    function loadList() {
                        fileTables.clear().draw();
                        fileTables.row.add([
                            '<i class="fas fa-folder"></i>',
                            "Home",
                            "/storage/emulated/0",
                            `<button type="button" data-toggle="tooltip" title="Abrir" data-path='/storage/emulated/0' class="btn btn-info btn-sm btn-margin act-open tooltip-download"><i class="fas fa-folder-open"></i></button>`
                        ]).draw(false);

                        $.getJSON(`/api/device/${window.deviceId}/dirs`,
                            function(dirs) {
                                if (dirs.length > 1) {
                                    for (var dir of dirs) {
                                        fileTables.row.add([
                                            dir.isDir ? '<i class="fas fa-folder"></i>' : '<i class="fas fa-file"></i>',
                                            dir.name,
                                            dir.path,
                                            dir.isDir
                                            ? `<button type="button" data-toggle="tooltip" title="Expandir pasta" data-path="${
                                            dir.path
                                            }" class="btn btn-info btn-sm btn-margin act-open tooltip-download"><i class="fas fa-folder-open"></i></button>`
                                            : `<button type="button" data-toggle="tooltip" title="Download" data-path="${
                                            dir.path
                                            }" class="btn btn-info btn-sm btn-margin act-download tooltip-download"><i class="fas fa-cloud-download-alt"></i></button>`
                                        ]).draw(false);
                                    }
                                } else {
                                    fileTables.row.add([
                                        '<i class="fas fa-ban"></i>',
                                        "Acesso Negado",
                                        " -- ",
                                        " -- "
                                    ]).draw(false);
                                }

                                $('.act-open').off('click');
                                $('.act-download').off('click');
                                $('.act-open').on('click',
                                    function() {
                                        var $this = $(this);
                                        loadDir($this.data('path'));
                                    });
                                $('.act-download').on('click',
                                    function() {
                                        var $this = $(this);
                                        downloadFile($this.data('path'));
                                    });

                                $('.tooltip-download').tooltip({
                                    container: "#dataTable_2_wrapper"
                                });

                            });
                    }

                    loadList();

                    function loadDir(path) {
                        toastr.info("Aguarde, carregando!");
                        $.ajax({
                            type: 'POST',
                            url: `/api/device/${window.deviceId}/dir`,
                            data: JSON.stringify({
                                path: path,
                                isDir: true
                            }),
                            success: function(data) {

                                var checkPathInterval = setInterval(function() {
                                        $.getJSON(`/api/device/${window.deviceId}/isLoading`,
                                            function(isLoading) {
                                                if (isLoading == false) {
                                                    loadList();
                                                    clearInterval(checkPathInterval);
                                                }
                                            });
                                    },
                                    5000);
                            },
                            contentType: "application/json",
                            dataType: 'json'
                        });
                    }


                    function downloadFile(path) {
                        toastr.info("Aguarde, carregando!");
                        $.ajax({
                            type: 'POST',
                            url: `/api/device/${window.deviceId}/dir`,
                            data: JSON.stringify({
                                path: path,
                                isDir: false
                            }),
                            success: function (data) {
                                toastr.success(
                                    "Comando enviado com sucesso, confira a aba Downloads em alguns instantes.");
                            },
                            error: function(data) {
                                toastr.error("Erro ao comunicar com o aparelho, tente novamente!");
                            },  
                            contentType: "application/json",
                            dataType: 'json'
                        });
                    }
                });
            }
        );
    }
);