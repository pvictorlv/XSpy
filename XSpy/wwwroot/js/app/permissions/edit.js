require(['/js/common.js'],
    function(common) {
        require([
                'jquery',
                'duallistbox',
                'jquery-validate', 'toastr', 'utils'
            ],
            function($, duallistbox, validate, toastr, utils) {
                $(document).ready(function() {
                    $('.dual_select').bootstrapDualListbox(utils.getDefaultDualSelectOptions());
                });

                $("#addUserForm").submit(function(e) {
                    e.preventDefault();
                    const form = $(this);

                    if (!form.valid()) {
                        toastr.error("Hey, verifique todos os campos!");
                        return false;
                    }

                    let url = form.attr('action');

                    var formData = utils.getFormData(form);
                    formData.rankId = window.rankId;
                    //formData.roles = formData["roles[]"];
                    //formData.fakeRoles = formData["fakeRoles[]"];

                    $.ajax({
                        type: "PATCH",
                        url: url,
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify(formData), // serializes the form's elements.
                        success: function(data) {
                            toastr.success("Editado com sucesso!");
                            setTimeout(function() {
                                    window.location.href = '/permissions/list';
                                },
                                2000);
                        },
                        error: function(data) {
                            toastr.error("Erro, tente novamente!");
                        }
                    });
                });

            });
    });