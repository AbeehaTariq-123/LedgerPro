const paymentToken = () => $('input[name="__RequestVerificationToken"]').val();
function loadPayments() { $.get('/Payment/GetAll').done(rows => { const body = $('#paymentTable tbody').empty(); rows.forEach(p => body.append(`<tr><td>${p.paymentId}</td><td>${p.paymentType}</td><td>${p.amount}</td><td>${new Date(p.paymentDate).toLocaleDateString()}</td></tr>`)); }); } $(loadPayments);
