require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function() {

                    $('#quickactions>[data-toggle="tooltip"]').tooltip({
                        container: "#quickactions"
                    });

                    $('#dataTable_wrapper').find('[data-toggle="tooltip"]').tooltip({
                        container: "#dataTable_wrapper"
                    });


                    $('#dataTable').DataTable({
                        serverSide: false
                    });
                    var fileTables = $('#dataTable_2').DataTable({
                        serverSide: false
                    });

                    function loadList() {
                        fileTables.clear().draw();
                        fileTables.row.add([
                            '<i class="fas fa-folder"></i>',
                            "Home",
                            "/storage/emulated/0",
                            `<button type="button" data-toggle="tooltip" title="Abrir" data-path='/storage/emulated/0' class="btn btn-info btn-sm btn-margin act-open"><i class="fas fa-folder-open"></i></button>`
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
                                            }" class="btn btn-info btn-sm btn-margin act-open"><i class="fas fa-sign-in-alt"></i></button>`
                                            : `<i class="fas fa-cloud-download-alt"></i>`
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
                                $('.act-open').on('click',
                                    function () {
                                        var $this = $(this);
                                        loadDir($this.data('path'));
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
                            success: function (data) {

                                var checkPathInterval = setInterval(function () {
                                        $.getJSON(`/api/device/${window.deviceId}/isLoading`,
                                            function(isLoading ) {
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
                });
            }
        );
    }
);