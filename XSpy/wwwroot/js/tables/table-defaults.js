define(["datatables.net-responsive", "jquery", "toastr", "moment"],
    function (dTables, $, toastr, moment) {

        function initTooltip() {
            $('[data-toggle="tooltip"]').tooltip({
                container: "#dataTable_wrapper"
            });
        }

        $(document).ready(function () {
            $("#sendSearch").on('click',
                function () {
                    window.dataTable.draw(false);
                });

            $(document).keypress(
                function (event) {
                    if (event.which == '13' || event.key === 'Enter') {
                        event.preventDefault();
                        window.dataTable.draw(false);
                    }
                });
        });

        function download(data, filename, type) {
            var file = new Blob([data], { type: type });
            if (window.navigator.msSaveOrOpenBlob) // IE10+
                window.navigator.msSaveOrOpenBlob(file, filename);
            else { // Others
                var a = document.createElement("a"),
                    url = URL.createObjectURL(file);
                a.href = url;
                a.download = filename;
                document.body.appendChild(a);
                a.click();
                setTimeout(function () {
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(url);
                },
                    0);
            }
        }

        if (window.exportUrl) {
            $.fn.dataTable.ext.buttons.exportXml = {
                text: 'XML',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: "POST",
                        url: window.exportUrl,
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify(window.lastSearch), // serializes the form's elements.
                        success: function (data) {
                            download(data, `Relatório - ${moment().format()}.xml`, "application/xml");
                        },
                        error: function (data) {
                            download(data.responseText, `Relatório - ${moment().format()}.xml`, "application/xml");
                            toastr.success("Exportado com sucesso!");
                        }
                    });
                }
            };
        }




        function setExportOptions(exportData) {

            if ($.fn.dataTable) {

                if (typeof exportData == 'undefined') {
                    exportData = null;
                }

                $.extend(true,
                    $.fn.dataTable.defaults,
                    {
                        language: {
                            url: "/locales/datatables-ptBR.json"
                        },
                        pageLength: 5,
                        "initComplete": function (settings, json) {
                            $("#dataTable_filter>label>input").attr('placeholder', 'Digite para pesquisar');
                        },
                        dom: '<"html5buttons"B>lfgtip',
                        lengthMenu: [[5, 10, 25, 50], [5, 10, 25, 50]],
                        processing: true,
                        serverSide: true,
                        info: false,
                        responsive: true,
                        orderMulti: true,
                        columnDefs: [
                            {
                                defaultContent: "N/A",
                                targets: "_all"
                            }
                        ],

                        buttons: [
                            {
                                extend: 'csv',
                                exportOptions: exportData
                            },
                            {
                                extend: 'excel',
                                exportOptions: exportData
                            },
                            {
                                extend: 'pdf',
                                customize: function (doc) {
                                    doc.styles.tableBodyEven.alignment = 'center';
                                    doc.styles.tableBodyOdd.alignment = 'center';
                                },
                                exportOptions: exportData
                            },
                            {
                                extend: 'print',
                                customize: function (win) {
                                    $(win.document.body).addClass('white-bg');
                                    $(win.document.body).css('font-size', '10px');

                                    $(win.document.body).find('table')
                                        .addClass('compact')
                                        .css('font-size', 'inherit');
                                },
                                exportOptions: exportData
                            }
                        ]
                    });

            }
        }

        return {
            initTooltip: initTooltip,
            setExportOptions: setExportOptions
        }
    });