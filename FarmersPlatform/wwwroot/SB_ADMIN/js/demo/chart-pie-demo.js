// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

var _0To50Element = document.getElementById("OrderPrice0To50");
var _0To50Percentage = JSON.parse(_0To50Element.getAttribute("0To50"));

var _50To100Element = document.getElementById("OrderPrice50To100");
var _50To100Percentage = JSON.parse(_50To100Element.getAttribute("50To100"));

var _100PlusElement = document.getElementById("OrderPrice100Plus");
var _100PlusPercentage = JSON.parse(_100PlusElement.getAttribute("100Plus"));

// Pie Chart Example
var ctx = document.getElementById("myPieChart");
var myPieChart = new Chart(ctx, {
    type: 'doughnut',
    data: {
        labels: ["%", "%", "%"],
        datasets: [{
            data: [_0To50Percentage, _50To100Percentage, _100PlusPercentage],
            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
            hoverBorderColor: "rgba(234, 236, 244, 1)",
        }],
    },
    options: {
        maintainAspectRatio: false,
        tooltips: {
            backgroundColor: "rgb(255,255,255)",
            bodyFontColor: "#858796",
            borderColor: '#dddfeb',
            borderWidth: 1,
            xPadding: 15,
            yPadding: 15,
            displayColors: false,
            caretPadding: 10,
        },
        legend: {
            display: false,
            position: 'bottom', // Display the legend at the bottom
            
        },
        cutoutPercentage: 80,
    },
});