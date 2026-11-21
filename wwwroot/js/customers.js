// Requires jQuery and an anti-forgery token in the page: @Html.AntiForgeryToken()
const customerToken = () => $('input[name="__RequestVerificationToken"]').val();
function loadCustomers() { $.get('/Customer/GetAll').done(rows => { const body = $('#customerTable tbody').empty(); rows.forEach(c => body.append(`<tr><td>${c.customerId}</td><td>${$('<div>').text(c.name).html()}</td><td>${$('<div>').text(c.phone || '').html()}</td><td>${$('<div>').text(c.email || '').html()}</td><td><button type="button" onclick="deleteCustomer(${c.customerId})">Delete</button></td></tr>`)); }).fail(() => alert('Could not load customers.')); }
function deleteCustomer(id) { if (!confirm('Delete this customer?')) return; $.post('/Customer/Delete', { id, __RequestVerificationToken: customerToken() }).done(() => loadCustomers()).fail(xhr => alert(xhr.responseText || 'Delete failed.')); }
$(loadCustomers);
