using Handus;

var view = new ConsoleView();
var presenter = new Presenter(view);

await presenter.Run();