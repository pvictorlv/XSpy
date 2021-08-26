define(['jquery', 'jquery-validate', 'utils', 'toastr'],
    function($, validate, utils, toastr) {
        let loaded = false;
        let editing = false;
        let lastId = null;
        
        function setupButtons() {
            if (window.canCreate == 'True') {
                $.fn.dataTable.defaults.buttons = [
                    {
                        text: 'Adicionar usuário',
                        className: 'btn-success',
                        action: function (e, dt, node, config) {
                            editing = false;

                            document.getElementById("userForm").reset();
                            $("#userModal").modal();
                            editing = false;
                        }
                    }, ...$.fn.dataTable.defaults.buttons
                ];
            }

            $("#saveBtn").click(function() {
                const form = $("#userForm");

                $("#saveBtn").attr('disabled', true);


                if (!form.valid()) {
                    $("#saveBtn").attr('disabled', false);
                    toastr.error("Hey, verifique todos os campos!");
                    return false;
                }

                toastr.info("Por favor, aguarde!");

                var formData = utils.getFormData(form);

                if (lastId != null) {
                    formData.userId = lastId;
                }

                $.ajax({
                    type: editing ? "PATCH" : "PUT",
                    url: "/api/user",
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify(formData),
                    success: function(data) {
                        if (editing)
                            toastr.success("Categoria editado com sucesso!");
                        else
                            toastr.success("Categoria criada com sucesso!");

                        $("#saveBtn").attr('disabled', false);

                        dataTable.draw(false);
                        $("#userModal").modal('hide');
                    },
                    error: function(data) {
                        if (!data || !data.responseJSON) {
                            toastr.error("Erro, tente novamente!");
                        } else {
                            toastr.error(data.responseJSON.message);
                        }

                        $("#saveBtn").attr('disabled', false);
                    }
                });

                return false;

            });
        }

        function init() {
            setupButtons();
        }

        function getUser(id) {
            $.ajax({
                type: "GET",
                url: '/api/user/' + id,
                contentType: "application/json",
                dataType: 'json',
                success: function (data) {
                    document.getElementById("userForm").reset();
                    editing = true;
                    $("#name").val(data.name);
                    lastId = data.id;
                    $("#userModal").modal('show');
                },
                error: function (data) {
                    toastr.error("Erro ao carregar!");
                }
            });
        }

        function onTableLoad() {
            $('.action-edit').click(function () {
                getUser($(this).data('id'));
            });
        }

        return {
            init: init,
            onTableLoad: onTableLoad
        }
    }
);