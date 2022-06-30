# ENG

## Program for generating combinational circuits
Software for generating combinational circuits based on given parameters.

### Оглавление
<a name="content_eng"></a> 
1. [Basic development parameters](#programmParameters_eng)
2. [Used packages](#packages_eng)
3. [List of generators](#generators_eng)

### Basic development parameters
<a name="programmParameters_eng"></a> 
The following parameters are required for the correct operation of the program:
- Microsoft Visual Studio 17.1.1
- .NETFramework,Version=v4.8

[&#8593; Contents](#content_eng)

### Used packages
<a name="packages_eng"></a> 
The following packages are required for the correct operation of the program:
- [ConsoleTables] - output tables to the console. Version: 2.4.2.
- [Newtonsoft] - interaction with JSON files. Version: 13.0.1.
- [NotificationWindow] - library for displaying pop-ups. Version: 1.1.38.

All of the packages listed can be installed through Manage NuGet Packages in Microsoft Visual Studio.

[&#8593; Contents](#content_eng)

### List of generators
<a name="generators_eng"></a> 
Descriptions of the generators implemented in the program will be added to this section:
- [X] Basic methods and classes for implementing generators
    - [X] Convert truth table to boolean expression
        - [X] To canonical normal forms
    - [X] Implementation of boolean expression parsing
    - [X] Implementation of a graph for storing a combinational circuit
    - [X] Generation of structural Verilog based on graph
    - [X] Implementation of the structure with the main logical elements, their designations and hierarchy level
    - [X] Implementation of the structure for storing the truth table
    - [X] Base class implementations for generating a combinational circuit dataset
        - [X] Generation using random truth table generator
    - [X] Implementation of a class for storing a combinational circuit
- [X] Implementation of combinational circuit generators
    - [X] Based on truth tables:
        The simplest method of generating a combinational circuit is the generation of a truth table of the circuit and its subsequent processing. To perform schema generation, you need to follow several steps:
        1) generation of a truth table;
        2) construction of SDNF and/or SKNF;
        3) representation of the resulting logical expression in a given basis;
        4) synthesis of a combinational circuit in Verilog.
    - [X] Method of level-by-level random connection of elements among themselves
    - [X] With random connection of vertices
        - [X] Implementation of helper functions
        - [X] Main software implementation
    - [X] Genetic Algorithm
        - [X] Parent selection implementation
          - [X] Implementation of parent selection parameters class
          - [X] Class implementation with 5 types of parent selection
        - [X] Crossbreeding Implementation
          - [X] Crossover parameter class implementation
          - [X] Class implementation with 5 crossover types
        - [X] Mutation Implementation
          - [X] Mutation parameter class implementation
          - [X] Class implementation with 6 types of mutations
          - [X] Implementation of the main class for type selection and direct mutation
        - [X] Implementation of the selection of a new population
          - [X] Implementation of the new population selection parameter class
          - [X] Implementation of a class with a base type of selection of a new population
    - [X] Mechanism for adding and evaluating parameters of third-party schemes
- [X] Implementation of algorithms for assessing the reliability of combinational circuits
    - [X] Polynomial calculation implementation
    - [X] Connecting a third-party program Nadezhda

[&#8593; Contents](#content_eng)

---

# RUS

## Программа генерации комбинационных схем
Программное обеспечение для генерации комбинационных схем на основе заданных параметров.

### Оглавление
<a name="content"></a> 
1. [Основные параметры разработки](#programmParameters)
2. [Используемые Packages](#packages)
3. [Список генераторов](#generators)

### Основные параметры разработки
<a name="programmParameters"></a> 
Для корректной работы необходимы:
- Microsoft Visual Studio 17.1.1
- .NETFramework,Version=v4.8

[&#8593; Оглавление](#content)

### Используемые Packages
<a name="packages"></a> 
Для корректной работы необходимы следующие пакеты:
- [ConsoleTables] - вывод таблиц в консоль. Версия: 2.4.2.
- [Newtonsoft] - взаимодействие с файлами типа JSON. Версия: 13.0.1.
- [NotificationWindow] - библиотека для отображения всплывающих окон. Версия: 1.1.38.

Все перечисленные пакеты можно установить через "Управление пакетами NuGet" в Microsoft Visual Studio.

[&#8593; Оглавление](#content)

### Список генераторов
<a name="generators"></a> 
В данный раздел будут добавляться описания генераторов, реализованных в программе:
- [X] Основные методы и классы для реализации генераторов
    - [X] Перевод таблицы истинности в логическое выражение
        - [X] В канонические нормальные формы
    - [X] Реализация парсинга логического выражения
    - [X] Реализация графа для храния комбинационной схемы
    - [X] Генерация структурного Verilog на основе графа
    - [X] Реализация структуры с оснвоными логическими элементами, их обозначениями и уровня иерархии
    - [X] Реализации структуры для хранения таблицы истинности
    - [X] Реализации базового класса для генерации датасета комбинационной схемы
        - [X] Генерация с использованием генератора на основе случайной таблицы истинности
    - [X] Реализация класса для хранения комбинационной схемы
- [X] Реализация генераторов комбинационных схем
    - [X] На основе таблиц истинности:
        Наиболее простым методом генерации комбинационной схемы является генерация таблицы истинности схемы и ее последующая обработка. Для выполнения генерации схемы необходимо выполнить несколько шагов:
        1)  генерация таблицы истинности;
        2)  построение СДНФ и/или СКНФ;
        3)  представление полученного логического выражения в заданном базисе;
        4)  синтез комбинационной схемы в Verilog.
    - [X] Методом поуровнего случайного соединения элементов между собой
    - [X] Со случайным соединением вершин
        - [X] Реализация вспомогательных функций
        - [X] Основна программная реализация
    - [X] Генетический алгоритм 
        - [X] Реализация отбора родителей
          - [X] Реализация класса параметров отбора родителей
          - [X] Реализация класса с 5 типами отбора родителей
        - [X] Реализация скрещивания 
          - [X] Реализация класса параметров скрещивания
          - [X] Реализация класса с 5 типами скрещивания
        - [X] Реализация мутаций 
          - [X] Реализация класса параметров мутации
          - [X] Реализация класса с 6 типами мутаций
          - [X] Реализация основной класса для выбора типа и непосредственно мутации
        - [X] Реализация отбора новой популяции 
          - [X] Реализация класса параметров отбора новой популяции
          - [X] Реализация класса с базовым типом отбора новой популяции
    - [X] Механизм добавления и оценки параметров сторонних схем 
- [X] Реализация алгоритмов оценки надежности комбинационных схем
    - [X] Реализация вычисления полинома
    - [X] Подключение сторонней программы Nadezhda

[&#8593; Оглавление](#content)


[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)

   [ConsoleTables]: <https://github.com/khalidabuhakmeh/ConsoleTables>
   [Newtonsoft]: <https://www.newtonsoft.com/json>
   [NotificationWindow]: <https://github.com/Tulpep/Notification-Popup-Window>