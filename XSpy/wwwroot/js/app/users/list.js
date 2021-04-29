require(['/js/common.js'],
    function(common) {
        require(['jquery', 'tables', 'utils', 'toastr'],
            function($, tables, utils, toastr) {
                $(document).ready(function() {
                    
                    window.dataTable = $('#dataTable').DataTable({
                        "ajax": {
                            "contentType": 'application/json',
                            "data": function(d) {
                                d.filter = utils.getFormData($("#searchForm"));
                                window.lastSearch = d;
                                return JSON.stringify(d);
                            },
                            "url": '/api/user/list',
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
                            {
                                data: 'name'
                            },
                            {
                                data: 'email'
                            },
                            {
                                data: 'enabled',
                                render: utils.booleanStatusRenderer
                            },
                            {
                                data: 'rankName'
                            },
                            {
                                data: 'id',
                                "mRender": function(data, type, full) {
                                    var buttons = '';
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