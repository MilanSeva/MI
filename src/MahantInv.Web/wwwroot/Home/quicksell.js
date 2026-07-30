class QuickSellModel {
    constructor(ProductId, Quantity, Note, UsageDate) {
        this.ProductId = ProductId;
        this.Quantity = Quantity;
        this.Note = Note;
        this.UsageDate = UsageDate;
    }
}

class Common {
    static init() {
        Common.ProductSearchSelect2();
    }

    static async ProductSearchSelect2() {
        let response = await fetch(baseUrl + 'api/product/search', {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => { return response.json() });

        $('#QuickSellProduct').prepend('<option selected></option>').select2({
            placeholder: 'Search Product',
            closeOnSelect: true,
            allowClear: true,
            width: '100%',
            minimumResultsForSearch: 10,
            data: response,
            templateResult: function (repo) {
                if (repo.loading) {
                    return repo.name;
                }
                var $container = $(
                    "<div class='select2-result-repository clearfix'>" +
                    "<div class='select2-result-repository__avatar'><img src='" + repo.picturePath + "'></div>" +
                    "<div class='select2-result-repository__meta'>" +
                    "<div class='select2-result-repository__title'>" + repo.gujaratiName + "</div>" +
                    "<div class='select2-result-repository__description'>" + repo.description + "</div>" +
                    "<div class='select2-result-repository__statistics'>" +
                    "<div class='select2-result-repository__forks'>" + repo.size + "" + repo.unitTypeCode + "</div>" +
                    "<div class='select2-result-repository__stargazers'>" + repo.company + "</div>" +
                    "<div class='select2-result-repository__watchers'>" + repo.storage + "</div>" +
                    "</div>" +
                    "</div>" +
                    "</div>"
                );
                return $container;
            },
            templateSelection: function (repo) {
                return repo.name
            }
        });
    }

    static clearValidation() {
        $('#quickSellAlert').empty();
        $('#QuickSellQuantity').removeClass('is-invalid');
        $('.select2-selection').removeClass('is-invalid');
    }

    static showValidationError(message) {
        $('#quickSellAlert').html(
            '<div class="alert alert-danger py-2 mb-3" role="alert">' + message + '</div>'
        );
    }

    static validate(productId, quantity) {
        Common.clearValidation();
        let isValid = true;
        if (!productId) {
            $('.select2-selection').addClass('is-invalid');
            isValid = false;
        }
        if (!quantity || parseFloat(quantity) <= 0) {
            $('#QuickSellQuantity').addClass('is-invalid');
            isValid = false;
        }
        if (!isValid) {
            Common.showValidationError('Please select a product and enter a quantity greater than 0.');
        }
        return isValid;
    }

    static async QuickSell(mthis) {
        let productId = $('#QuickSellProduct').val();
        let quantity = $('#QuickSellQuantity').val();
        let note = $('#QuickSellNote').val();

        if (!Common.validate(productId, quantity)) {
            return;
        }

        let $btn = $(mthis);
        $btn.prop('disabled', true);

        try {
            let today = new Date();
            let usageDate = today.getFullYear() + '-' + String(today.getMonth() + 1).padStart(2, '0') + '-' + String(today.getDate()).padStart(2, '0');
            let quickSellModel = new QuickSellModel(productId, quantity, note, usageDate);

            let response = await fetch(baseUrl + 'api/product/usage', {
                method: 'POST',
                body: JSON.stringify(quickSellModel),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
            }).then(response => { return response.json() });

            if (response.success) {
                toastr.success('Saved', '', { positionClass: 'toast-top-center' });
                Common.clearValidation();
                $('#QuickSellProduct').val('').trigger('change');
                $('#QuickSellQuantity').val('');
                $('#QuickSellNote').val('');
                $('.select2-search__field').focus();
                if (typeof MyNotification !== 'undefined') {
                    MyNotification.GetPendingORNotifiedNotifications();
                }
            } else {
                var errorHtml = '';
                $.each(response.errors, function (index, element) {
                    errorHtml += element + ' ';
                });
                toastr.error(errorHtml, '', { positionClass: 'toast-top-center' });
            }
        } finally {
            $btn.prop('disabled', false);
        }
    }
}

jQuery(document).ready(function () {
    Common.init();
});
