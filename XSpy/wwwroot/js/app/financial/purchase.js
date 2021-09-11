require(['/js/common.js'],
    function(common) {
        require(['jquery', 'jquery-validate', 'card', 'toastr', 'utils', 'direct-checkout'],
            function ($, validate, card, toastr, utils, ds) {
                $(document).ready(function() {

                    let loadedVoucher = null;
                    $("#planId").change(function () {
                        loadPrice();
                    });

                    function loadPrice() {
                        var plans = document.getElementById('planId');
                        var selected = plans.options[plans.selectedIndex];

                        var price = selected.getAttribute('data-price');
                        $("#price").val("R$" + formatValue(price / 100));
                        $(".valueTotal").html("R$" + formatValue(price / 100));
                        $(".selectedProduct").html(selected.label);
                    }

                    loadPrice();

                    function formatValue(value) {
                        return parseFloat(value).toFixed(2);
                    }


                    $("#bankBtn").on('click',
                        function() {
                            toastr.info("Por favor, aguarde!");
                            var quantity = $("#planId").val();
                            if (!quantity) {
                                toastr.error("Selecione um plano!");
                                return;
                            }


                            var url = '/api/financial/purchase/deposit';
                            if (loadedVoucher) {
                                url += '?voucher=' + loadedVoucher.voucherCode;
                            }

                            $("#bankBtn").prop('disabled', true);
                            $.ajax({
                                type: "POST",
                                contentType: "application/json",
                                dataType: 'json',
                                data: JSON.stringify({
                                    planId: quantity
                                }),
                                url: url
                            }).done(function(data) {
                                if (window.fbq)
                                    window.fbq('track', 'Purchase', { value: quantity / 100, currency: 'BRL' });

                                toastr.success("Pedido realizado com sucesso!");
                                $("#digitableLine").html(`<pre>${atob(data.charges[0].pix.payloadInBase64)}</pre>`);
                                $("#bankBtn").prop('disabled', true);
                                $("#bankUrl").toggleClass("disabled");
                                $("#bankUrl").attr("href", data.charges[0].link);
                                $("#bankUrl").attr("target", "_blank");
                                window.open(data.charges[0].link);
                            }).fail(function(e) {
                                $("#bankBtn").prop('disabled', false);
                                toastr.error("Erro, tente novamente!");

                            });
                        });
                    $('#payment-form').card({
                        // a selector or DOM element for the container
                        // where you want the card to appear
                        container: '.card-wrapper', // *required*
                        placeholders: {
                            number: '•••• •••• •••• ••••',
                            name: 'Nome completo',
                            expiry: '••/••••',
                            cvc: '•••'
                        },
                        // all of the other options from above
                    });




                    $("#paymentBtn").click(function() {

                        const form = $("#payment-form");
                        if (!form.valid()) {
                            toastr.error("Hey, verifique todos os campos!");
                            return false;
                        }

                        toastr.info("Por favor, aguarde!");
                        var quantity = $("#planId").val();
                        if (!quantity) {
                            toastr.error("Selecione um plano!");
                            return false;
                        }
                        var formData = utils.getFormData(form);

                        var cardData = {
                            cardNumber: formData.number.replaceAll(' ', ''),
                                expirationMonth: formData.expiry.split(" / ")[0],
                                expirationYear:  formData.expiry.split(" / ")[1],
                                securityCode: formData.cvc,
                                holderName: formData.name
                        }
                        var checkout = new DirectCheckout('79387B2F638B8EE867A79804D26D660ED886B756DEDEA38147BCF2AD7C176A8E7BDC1B5F1FA7B7E8', true);

                        checkout.getCardHash(cardData, function (cardHash) {

                            var reqData = {
                                planId: $("#planId").val(),
                                cardHash: cardHash,
                                installments: 1
                        };

                            let voucherCode = $('#voucher-code').val();
                            reqData.voucherCode = voucherCode;

                            toastr.info("Por favor, aguarde!");
                            $("#paymentBtn").prop('disabled', true);
                            $.ajax({
                                type: "POST",
                                url: '/api/financial/purchase/card',
                                contentType: "application/json",
                                dataType: 'json',
                                data: JSON.stringify(reqData),

                                success: function (data) {
                                    if (data.paymentStatus != "Success") {
                                        toastr.error("Erro ao efetuar o pagamento!");
                                        $("#paymentBtn").prop('disabled', false);
                                        return;
                                    }
                                    
                                    toastr.success("Compra realizada com sucesso!");
                                    setTimeout(function () {
                                            window.location.href = '/orders/list';
                                        },
                                        1000);
                                },
                                error: function (data) {
                                    $("#paymentBtn").prop('disabled', false);
                                    toastr.error("Erro, tente novamente!");
                                }
                            });
                        }, function (error) {
                            toastr.error(error);
                        });

                        return false;
                    });


                    $('#voucher-button').click(function() {
                        loadedVoucher = null;
                        let voucherCode = $('#voucher-code').val();
                        if (voucherCode) {
                            $.getJSON('/api/voucher?code=' + voucherCode, applyVoucher)
                                .fail(function() { toastr.error("Cupom inválido"); });
                        } else {
                            toastr.error('Insira o código do cupom!');
                        }
                    });

                    function applyVoucher(voucher) {
                        if (!voucher) {
                            toastr.error('Cupom inválido!');
                        } else if (voucher.isExpired) {
                            toastr.warning('Cupom expirado!');
                        } else if (voucher.allUsed) {
                            toastr.warning('Cupom esgotado!');
                        } else {
                            loadedVoucher = voucher;
                            sumPrice();
                            toastr.success('Cupom aplicado!');
                        }
                    }
                });


            });
    }
);