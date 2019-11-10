# Google Cloud Toolbox

I want to test WPF on .net core. So I created this small tool to interact with some of the services that I use on Google Cloud.

Currenty it allows to view and edit kubernetes secrets.

To run the tool you must have `kubectl` configured.

Since my main goal was just to see how WPF is doing in .net core, some error handling is missing. The code also lack unit tests. Some synchronization is also required for fast intractions.

This animation shows the basic working of the application:

![](gcloud-toolbox.gif | width=700)

Many tabs can be opened at the same time:

![](gcloud-toolbox-1.gif | width=700)
