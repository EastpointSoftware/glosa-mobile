# GLOSA Mobile

The Green Light Optimal Speed Advisory (GLOSA) project is lead by West Midlands Combined Authority in partnership with Birmingham City Council, Amey, Integrated Design Techniques Limited (IDT) and SG Transport Innovation International.

The app itself is a bespoke Xamarin solution that integrates with road-side systems, providing analysis and predictions about the optimal approach speed to traffic lights.

This information will allow drivers to make decisions that can minimise red light stops, providing better traffic flow and reducing emissions from vehicles stopping, waiting, and starting.

## Getting Started

Clone the project to your local machine and open with Visual Studio (Windows or MacOS).

### Prerequisites

You must have the Xamarin development Android 5.1(Lollipop API 22).
A full guide on installing and configuring Xamarin is available at [developer.xamarin.com](https://developer.xamarin.com/guides/cross-platform/getting_started/installation/)

### Running the app

Android Device running Android > 5.1

```
Reccommend to use device with Lollipop >= 5.1 rather than emulator.
```

## Running the tests

The test project is called GreenLight.Tests.

### Break down into end to end tests

Intergration testing

```
TODO
```
## CROCS - Controller to RSU Open C-ITS SCHEMA

* [Schema Dictionary](http://www.idtuk.com/downloads/crocs/schema_0-1/CROCS-Data-Dictionary-0-1.pdf) - CROCS Data Dictionary

The above document defines the 2 message types used by the application:
* MAP - Junction Topology
* SPAT - Signal Phase and Timing

The whole schema is beyond the scope of this README but below is the subset of the schema used by the application.

### SPAT

### MAP


## Built With

* [Xamarin](https://developer.xamarin.com) - Cross platform development software
* [Visual Studio](https://www.visualstudio.com/) - IDE
* [Azure](https://portal.azure.com/) - Cloud Services
* [PowerBI](https://powerbi.com/) - Data Visualisation

## Contributing

Please read [CONTRIBUTING.md](https://eastpoint.visualstudio.com/_git/Project%20Green%20Light?path=%2FCONTRIBUTING.md&version=GBdevelopment&_a=preview) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Jin Khera** - *Initial work* - [Jin Khera](http://www.eastpoint.co.uk/eastpoint-team/)
* **Alice Lai** - *Initial work* - [Alice Lai](http://www.eastpoint.co.uk/eastpoint-team/)
* **Oliver Gilmore** - *Initial work* - [Oliver Gilmore](http://www.eastpoint.co.uk/eastpoint-team/)

See also the list of [contributors](http://www.eastpoint.co.uk/eastpoint-team/) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* West Midlands Combined Authority 
* Birmingham City Council
* Amey
* Integrated Design Techniques Limited (IDT)
* SG Transport Innovation International
* Front-end team (Alice, Oliver and Jin)
* Back-end team (Alice, Oliver and Jin)

