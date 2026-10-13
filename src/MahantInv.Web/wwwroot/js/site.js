// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Category multi-select: typeahead from api/categories/search, and any typed name can be added as a new tag.
// Option values are category names; api/product/save maps them to existing categories or creates new ones.
function initCategorySelect2($select, $dropdownParent) {
    $select.select2({
        dropdownParent: $dropdownParent,
        placeholder: 'Search or add Category',
        closeOnSelect: true,
        tags: true,
        ajax: {
            url: baseUrl + 'api/categories/search',
            dataType: 'json',
            delay: 250,
            data: params => ({ query: params.term || '' }),
            processResults: data => ({ results: data.map(c => ({ id: c.name, text: c.name })) })
        },
        createTag: params => {
            let term = $.trim(params.term);
            return term === '' ? null : { id: term, text: term, newTag: true };
        },
        // Don't offer "Foo (new)" when the search already returned "foo"
        insertTag: (data, tag) => {
            let exists = data.some(d => (d.text || '').toLowerCase() === tag.text.toLowerCase());
            if (!exists) data.unshift(tag);
        },
        templateResult: data => data.newTag ? $('<span>').text(data.text + ' (new)') : data.text
    });
}

// Fills the Product Name / Gujarati Name <datalist>s of an Add/Edit Product modal from a list of products
// ({ name, gujaratiName }), so typing shows existing names as suggestions. Browser-native, so no extra library.
function fillProductNameSuggestions($modal, products) {
    const fill = (selector, values) => {
        const unique = [...new Set(values.map(v => (v || '').trim()).filter(v => v !== ''))]
            .sort((a, b) => a.localeCompare(b));
        $modal.find(selector).empty().append(unique.map(v => $('<option>').attr('value', v)));
    };
    fill('#ProductNameSuggestions', products.map(p => p.name));
    fill('#ProductGujaratiNameSuggestions', products.map(p => p.gujaratiName));
}
