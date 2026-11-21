const supplierToken = antiForgeryToken;

function loadSuppliers() {
    $.get('/Supplier/GetAll').done(rows => {
        const body = $('#supplierTable tbody').empty();
        if (!rows.length) {
            body.append('<tr><td colspan="6" class="text-center">No suppliers yet.</td></tr>');
            return;
        }
        rows.forEach(s => body.append(`
            <tr>
                <td>${s.supplierId}</td>
                <td><strong>${esc(s.name)}</strong></td>
                <td>${esc(s.phone)}</td>
                <td>${esc(s.email)}</td>
                <td>PKR ${money(s.openingBalance)}</td>
                <td class="text-right">
                    <div class="table-actions">
                        <a class="btn btn-sm btn-outline" href="/Supplier/Ledger/${s.supplierId}">Ledger</a>
                        <a class="btn btn-sm btn-secondary" href="/Supplier/Edit/${s.supplierId}">Edit</a>
                        ${window.LedgerPro && LedgerPro.isAdmin
                            ? `<button type="button" class="btn btn-sm btn-terracotta" onclick="deleteSupplier(${s.supplierId})">Delete</button>`
                            : ''}
                    </div>
                </td>
            </tr>`));
    }).fail(() => alert('Could not load suppliers.'));
}

function deleteSupplier(id) {
    if (!confirm('Delete this supplier?')) return;
    $.post('/Supplier/Delete', { id, __RequestVerificationToken: supplierToken() })
        .done(() => loadSuppliers())
        .fail(xhr => alert(xhr.responseText || 'Delete failed.'));
}

$(loadSuppliers);
