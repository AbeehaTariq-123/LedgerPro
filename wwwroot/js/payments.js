function loadPayments() {
    $.get('/Payment/GetAll').done(rows => {
        const body = $('#paymentTable tbody').empty();
        if (!rows.length) {
            body.append('<tr><td colspan="5" class="text-center">No payments yet.</td></tr>');
            return;
        }
        rows.forEach(p => {
            const received = p.paymentType === 'Received';
            body.append(`
            <tr>
                <td>${fmtDate(p.paymentDate)}</td>
                <td><span class="badge ${received ? 'badge-paid' : 'badge-credit'}">${received ? 'Received' : 'Paid'}</span></td>
                <td><strong>${esc(p.partyName)}</strong></td>
                <td class="${received ? 'amount-in' : 'amount-out'}">PKR ${money(p.amount)}</td>
                <td>${esc(p.description)}</td>
            </tr>`);
        });
    }).fail(() => alert('Could not load payments.'));
}

$(loadPayments);
