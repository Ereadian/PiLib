# PiLib
Rasberry PI Library

This library is built on top of WiringPi library. To setup wiringPi library, please:
step 1: Get the source code from repository. 
	git clone git://git.drogon.net/wiringPi

step 2:Install
	cd wiringPi
	git pull origin
	./build
	
To use this library, you need mono IDE and runtime.
This libibrary has two parts: remote host (PiConnector) and library (PiHardware)
you need to run the PiConnector as admin:
	sudo PiConnector
the connector accepts an optional parameter to set the port. If not specified, 5555 will be used.
	sudo PiConnector 777

You can reference to the PiHardware. By create RemoteGpio you can manage your Raspberry Pi either from local or remote from another Pi or computer using C#.
