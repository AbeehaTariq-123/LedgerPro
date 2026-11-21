// Shared helpers for LedgerPro pages (requires jQuery).
const esc = s => $('<div>').text(s ?? '').html();
const money = n => Number(n ?? 0).toLocaleString('en-PK', { maximumFractionDigits: 2 });
const fmtDate = d => new Date(d).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
// There can be more than one anti-forgery input on a page (logout form + page form); any of them is valid.
const antiForgeryToken = () => $('input[name="__RequestVerificationToken"]').first().val();

// Live search for the AJAX-loaded tables: filters the rows already fetched from the server.
$(document).on('input', '#tableSearch', function () {
    const q = this.value.toLowerCase();
    $('.table-wrapper tbody tr').each(function () {
        $(this).toggle($(this).text().toLowerCase().includes(q));
    });
});
