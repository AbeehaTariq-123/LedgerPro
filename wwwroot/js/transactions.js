function loadTransactions() {
    $.get('/Transaction/GetAll').done(rows => {
        const body = $('#transactionTable tbody').empty();
        if (!rows.length) {
            body.append('<tr><td colspan="5" class="text-center">No transactions yet.</td></tr>');
            return;
        }
        rows.forEach(t => {
            const badge = ['Received', 'Paid', 'Payment', 'Receipt'].includes(t.transactionType) ? 'badge-paid' : 'badge-credit';
            body.append(`
            <tr>
                <td>${fmtDate(t.transactionDate)}</td>
                <td><span class="badge ${badge}">${esc(t.transactionType)}</span></td>
                <td><strong>${esc(t.partyName)}</strong></td>
                <td>PKR ${money(t.amount)}</td>
                <td>${esc(t.description)}</td>
            </tr>`);
        });
    }).fail(() => alert('Could not load transactions.'));
}

$(loadTransactions);
