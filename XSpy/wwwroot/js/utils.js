define(["moment", "toastr"],
    function(moment, toastr) {

        function getFormData($form) {
            return $form.serializeArray().reduce(function(newData, item) {
                    // Treat Arrays
                    item.name = item.name.replace('order_search[', '');
                    if (item.name.substring(item.name.length - 2) === '[]') {
                        var key = item.name.substring(0, item.name.length - 2);
                        if (typeof (newData[key]) === 'undefined') {
                            newData[key] = [];
                        }
                        newData[key].push(item.value);
                    } else {
                        newData[item.name] = item.value;
                    }
                    return newData;
                },
                {});

        }

        function dateRenderer(data, type, row, meta) {
            if (!data) {
                return 'N/A';
            }

            return moment(data).format("DD/MM/YY HH:mm");
        }

        function utcDateRenderer(data, type, row, meta) {
            if (!data) {
                return 'N/A';
            }

            return moment.utc(data).format("DD/MM/YYYY HH:mm:ss");
        }

        function booleanRenderer(data, type, row, meta) {
            if (!data) {
                return '<span class="label label-danger">Não</span>';
            }

            if (data == true || data == 'True') {
                return '<span class="label label-primary">Sim</span>';
            }

            return '<span class="label label-danger">Não</span>';
        }

        function booleanStatusRenderer(data, type, row, meta) {
            if (!data) {
                return '<span class="label label-danger">Desativado</span>';
            }

            if (data == true || data == 'True') {
                return '<span class="label label-primary">Ativado</span>';
            }

            return '<span class="label label-danger">Desativado</span>';
        }
        function transactionStatusRenderer(data, type, row, meta) {
            if (!data) {
                return '<span class="label label-danger">Indefinido</span>';
            }

            switch (data) {
            case 'Success':
                return '<span class="label label-primary">Sucesso</span>';

            case 'Failed':
                return '<span class="label label-danger">Falhou</span>';

            case 'Pending':
                return '<span class="label label-warning">Pendente</span>';
            default:
                return '<span class="label label-danger">Cancelada</span>';
            }
        }

        function paymentTypeRenderer(data, type, row, meta) {

            switch (data) {
            case 'Deposit':
                return '<span class="label label-success">Depósito / Boleto</span>';

            case 'CreditCard':
                return '<span class="label label-success">Cartão de crédito</span>';
            default:
                return '<span class="label label-danger">Indefinido</span>';
            }
        }


        function defaultResponseErrorHandler(e) {
            if (!e.responseJSON.message) {
                toastr.error("Erro, tente novamente!");
            } else {
                toastr.error(e.responseJSON.message);
            }
        }


        function floatRender(data, type, row, meta) {
            if (!data) {
                return '0.00';
            }

            return parseFloat(data).toFixed(2);
        }

        return {
            defaultResponseErrorHandler: defaultResponseErrorHandler,
            getFormData: getFormData,
            booleanStatusRenderer: booleanStatusRenderer,
            floatRender: floatRender,
            dateRenderer: dateRenderer,
            booleanRenderer: booleanRenderer,
            utcDateRenderer: utcDateRenderer,
            paymentTypeRenderer: paymentTypeRenderer,
            transactionStatusRenderer: transactionStatusRenderer
        }
    });