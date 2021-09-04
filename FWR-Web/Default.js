$('.header').click(function () {

	$(this).nextUntil('tr.header').slideToggle(1000);
});