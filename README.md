# Implementacja prostego silnika Dependency Injection/Inversion of Control

Zadanie realizowane w ramach przedmiotu Projektowanie Obiektowe Oprogramowania na Uniwersytecie Wrocławskim.
Zostało ono wykonane w maju 2021.

### Dostępne metody:

* Resolve - tworzy instancje obiektu danego typu.
* RegisterType - pozwala zarejestrować politykę tworzenia instancji obiektów. Jedno przeciążenie pozwala wybrać czy stosujemy politykę singleton. Drugie pozwala zarejestrować implementację klasy bazowej, abstrakcyjnej lub interfejsu.
* RegisterInstance - pozwala ustawić konktretną instancję obiektu na tą która będzie zwracana.
* BuildUp - pozwala uzupełnić zależności istniejącej już instancji obiektu. Wstrzykuje ona metody i właściwości.

Silnik zawiera mechanizm Dependency Injection. Podczas próby utworzenia instancji obiektu, silnik wybiera konstruktor o największej liczbie argumentów lub ten oznaczony atrybutem [DependencyConstrutor], jeśli jest tylko jeden taki. Próbuje on rekurencyjnie utworzyć argumenty dla konstruktora.

Silnik wstrzykuje również metody i właściwości, jeśli są one oznaczone specjalnymi atrybutami. Rekurencyjnie próbuje on utworzyć instancje potrzebnych obiektów (argumentów lub typu właściwości).
