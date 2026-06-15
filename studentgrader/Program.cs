using studentgrader.services;
using studentgrader.ui;


var service = new GradeService();
var ui      = new ConsoleUI(service);
ui.Run();

