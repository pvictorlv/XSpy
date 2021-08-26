require(['/js/common.js'],
    function(common) {
        require(['main', 'ichecks', 'select2', 'jasny-bootstrap', 'jquery-validate', 'card', 'toastr', 'utils'],
            function(main, icheck, select2, jasny, validate, card, toastr, utils) {
                $(document).ready(function() {

                    let loadedVoucher = null;
                    $("#quantity").on('input',
                        function() {
                            sumPrice();
                        });

                    function sumPrice() {
                        var price = getPrice();
                        $(".valueTotal").html('R$' + price);

                        let finalValue = price;
                        if (loadedVoucher) {
                            if (loadedVoucher.type === "FixedValue") {
                                $(".valueDiscount").html('R$' + formatValue(loadedVoucher.value));
                                finalValue = (window.pricePerCredit * $("#quantity").val()) - loadedVoucher.value;
                                $(".valueWithDiscount")
                                    .html('R$' +
                                        formatValue(finalValue));

                            } else {
                                var quantity = $("#quantity").val();
                                var discountPercent = loadedVoucher.value;
                                if (loadedVoucher.rewardIncreaseValue > 0 &&
                                    quantity >= loadedVoucher.rewardIncreaseEach) {
                                    var increaseSum = Math.floor(quantity / loadedVoucher.rewardIncreaseEach);
                                    if (increaseSum > 0) {
                                        discountPercent = loadedVoucher.value +
                                            (loadedVoucher.rewardIncreaseValue * increaseSum);
                                    }
                                }

                                let discount = discountPercent * price;
                                if (discount > loadedVoucher.maxValue) {
                                    discount = loadedVoucher.maxValue;
                                }

                                $(".valueDiscount").html('R$' + formatValue(discount));
                                finalValue = (window.pricePerCredit * quantity) - discount;

                                $(".valueWithDiscount").html('R$' + formatValue(finalValue));

                            }
                        }
                        var installments = $("#cardInstallments");
                        installments.find('option')
                            .remove()
                            .end();

                        installments.append(new Option(`1 x R$${finalValue}`, 1, true, true));
                        var floatFinal = parseFloat(finalValue);
                        for (var i = 2; i <= 12; i++) {
                            var installmentTax = i - 1;
                            var taxVal = (((window.taxPerInstallment * installmentTax) * floatFinal) + floatFinal) / i;

                            installments.append(new Option(`${i} x R$${formatValue(taxVal)}`, i));
                        }

                    }

                    function formatValue(value) {
                        return parseFloat(value).toFixed(2);
                    }

                    function getPrice() {
                        return formatValue(window.pricePerCredit * $("#quantity").val());
                    }

                    $("#bankBtn").on('click',
                        function() {
                            toastr.info("Por favor, aguarde!");
                            var quantity = $("#quantity").val();
                            if (!quantity) {
                                toastr.error("Insira uma quantidade!");
                                return;
                            }


                            var url = '/api/boleto/' + quantity;
                            if (loadedVoucher) {
                                url += '?voucher=' + loadedVoucher.voucherCode;
                            }

                            $("#bankBtn").prop('disabled', true);
                            $.ajax({
                                type: "GET",
                                url: url
                            }).done(function(data) {
                                if (window.fbq)
                                    window.fbq('track', 'Purchase', { value: quantity / 100, currency: 'BRL' });

                                toastr.success("Pedido realizado com sucesso!");
                                $("#digitableLine").html(`<pre>${data.digitable_line}</pre>`);
                                $("#bankBtn").prop('disabled', true);
                                $("#bankUrl").toggleClass("disabled");
                                $("#bankUrl").attr("href", data.url_slip_pdf);
                                $("#bankUrl").attr("target", "_blank");
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
                            expiry: '••/••',
                            cvc: '•••'
                        },
                        // all of the other options from above
                    });

                    $('.i-checks').iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green',
                    });

                    $(".select2").select2({
                        placeholder: " Selecione!"
                    });

                    $("#roles").select2({
                        placeholder: " Selecione o cargo do usuário",
                        allowClear: true
                    });

                    $("#searchSchoolId").select2({
                        placeholder: " Selecione a escola do usuário",
                        allowClear: true
                    });


                    $("#paymentBtn").click(function() {

                        const form = $("#payment-form");
                        if (!form.valid()) {
                            toastr.error("Hey, verifique todos os campos!");
                            return false;
                        }

                        var formData = utils.getFormData(form);
                        var reqData = {
                            amount: $("#quantity").val(),
                            creditCard: {
                                CardNumber: formData.number,
                                ExpirationDate: formData.expiry,
                                SecurityCode: formData.cvc,
                                Holder: formData.name
                            },
                        };

                        let voucherCode = $('#voucher-code').val();
                        reqData.voucherCode = voucherCode;

                        toastr.info("Por favor, aguarde!");
                        $("#paymentBtn").prop('disabled', true);
                        $.ajax({
                            type: "POST",
                            url: '/api/financial/purchase/credits',
                            contentType: "application/json",
                            dataType: 'json',
                            data: JSON.stringify(reqData),

                            success: function(data) {
                                if (data.paymentStatus != "Success") {
                                    toastr.error("Erro ao efetuar o pagamento!");
                                    $("#paymentBtn").prop('disabled', false);
                                    return;
                                }

                                if (window.fbq) {
                                    var price = getPrice();

                                    window.fbq('track', 'Purchase', { value: price, currency: 'BRL' });
                                }

                                toastr.success("Compra realizada com sucesso!");
                                setTimeout(function() {
                                        window.location.href = '/orders/list';
                                    },
                                    1000);
                            },
                            error: function(data) {
                                $("#paymentBtn").prop('disabled', false);
                                toastr.error("Erro, tente novamente!");
                            }
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