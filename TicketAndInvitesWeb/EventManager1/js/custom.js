function newValue() {
    var newName = document.getElementById("newTicketTypeValue").value;
    console.log(newName);
    document.getElementById('ticketType').innerHTML += "<label class='btn btn-secondary'>" +
        "<input type='radio' name='options' id='option1' autocomplete='off'> " + newName +
        "</label>";
}
