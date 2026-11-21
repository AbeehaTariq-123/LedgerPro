// Requires jQuery + site.js and an anti-forgery token on the page (the layout's logout form provides one).
const customerToken = antiForgeryToken;

function loadCustomers() {
    $.get('/Customer/GetAll').done(rows => {
        const body = $('#customerTable tbody').empty();
        if (!rows.length) {
            body.append('<tr><td colspan="6" class="text-center">No customers yet.</td></tr>');
            return;
        }
        rows.forEach(c => body.append(`
            <tr>
                <td>${c.customerId}</td>
                <td><strong>${esc(c.name)}</strong></td>
                <td>${esc(c.phone)}</td>
                <td>${esc(c.email)}</td>
                <td>PKR ${money(c.openingBalance)}</td>
                <td class="text-right">
                    <div class="table-actions">
                        <a class="btn btn-sm btn-outline" href="/Customer/Ledger/${c.customerId}">Ledger</a>
                        <a class="btn btn-sm btn-secondary" href="/Customer/Edit/${c.customerId}">Edit</a>
                        ${window.LedgerPro && LedgerPro.isAdmin
                            ? `<button type="button" class="btn btn-sm btn-terracotta" onclick="deleteCustomer(${c.customerId})">Delete</button>`
                            : ''}
                    </div>
                </td>
            </tr>`));
    }).fail(() => alert('Could not load customers.'));
}

function deleteCustomer(id) {
    if (!confirm('Delete this customer?')) return;
    $.post('/Customer/Delete', { id, __RequestVerificationToken: customerToken() })
        .done(() => loadCustomers())
        .fail(xhr => alert(xhr.responseText || 'Delete failed.'));
}

$(loadCustomers);
