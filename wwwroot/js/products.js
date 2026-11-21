const productToken = antiForgeryToken;

function loadProducts() {
    $.get('/Product/GetAll').done(rows => {
        const body = $('#productTable tbody').empty();
        if (!rows.length) {
            body.append('<tr><td colspan="7" class="text-center">No products yet.</td></tr>');
            return;
        }
        rows.forEach(p => {
            const low = p.stockQuantity <= p.reorderLevel;
            body.append(`
            <tr>
                <td>${p.productId}</td>
                <td><strong>${esc(p.productName)}</strong></td>
                <td>${esc(p.sku)}</td>
                <td>PKR ${money(p.purchasePrice)}</td>
                <td>PKR ${money(p.salePrice)}</td>
                <td><span class="badge ${low ? 'badge-overdue' : 'badge-paid'}">${money(p.stockQuantity)}${low ? ' · low' : ''}</span></td>
                <td class="text-right">
                    <div class="table-actions">
                        <a class="btn btn-sm btn-secondary" href="/Product/Edit/${p.productId}">Edit</a>
                        <button type="button" class="btn btn-sm btn-terracotta" onclick="deleteProduct(${p.productId})">Delete</button>
                    </div>
                </td>
            </tr>`);
        });
    }).fail(() => alert('Could not load products.'));
}

function deleteProduct(id) {
    if (!confirm('Delete this product?')) return;
    $.post('/Product/Delete', { id, __RequestVerificationToken: productToken() })
        .done(loadProducts)
        .fail(xhr => alert(xhr.responseText || 'Delete failed.'));
}

$(loadProducts);
