# Лог изменений

Все заметные изменения в этом проекте будут отражаться в этом документе.

Формат лога изменений базируется на [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [2.4.27] - 2024-03-06

### Добавлено

* возможность настраивать JSON сериализатор

## [2.4.26] - 2024-02-13

### Изменено

* опциональная возможность не включать тела в дампы запроса и ответа 
* валидация ответов сервера выделена в отдельный сервис с настройкой включения тел в дампы запросов/ответов

## [2.2.23] - 2024-02-06

### Удалено

* логирование дампов сообщений при ошибках взаимодействия с `Elasticsearch`

## [2.2.5] - 2023-04-24

### Изменено

* без обратной совместимости изменён интерфейс инструментария для работы см индексами `IesIndexTools`

### Добавлено

* инструментарий для работы с потоками `IEsStreamTools`
* инструментарий для работы с жизненными циклами `IEsLifecycleTools`
* инструментарий для работы с шаблонами компонентов `IEsComponentTemplateTools`

## [2.1.5] - 2022-07-08

### Добавлено

* у `EsException` возможность определить, что ошибка из-за отсутствия индекса  

## [2.0.5] - 2022-07-08

### Изменено

* полная переделка

## [1.6.22] - 2022-06-08

### Добавлено

* раздельные методы интеграции и конфигурирования
* метод конфигурирования в коде
* возможность указывать кастомный сериализатор
* поддержка `NewtonJson` сериализатора

## [1.5.21] - 2021-08-05

### Добавлено

* Создание индекса с настройками в виде json строки
* Предварительная проверка провайдера имени индекса по умолчанию перед его использованием в искателе и индексаторе
* В индексатор дополнительный метод индексирования с дескриптором `bulk` операций 
* Логирование запросов в `ElasticSearch`

### Изменено

* Наименование проекта `MyLab.Elastic` -> `MyLAb.Search.EsAdapter`

## [1.4.13] - 2020-12-12

### Добавлено

* Фильтр по умолчанию на уровне поисковика
* Сортировка по умолчанию на уровне поисковика
* Фильтр по умолчанию на уровне стратегии поиска
* Сортировка по умолчанию на уровне стратегии поиска
* Поддержка создания контекстных поисковика и индексатора для индекса по умолчанию

### Изменено

* Переименованы методы добавления именованных фильтров и сортировок в поисковом движке `EsSearchEngine<TDoc>`

## [1.3.13] - 2020-12-09

### Добавлено

* Поддержка частичного обновления документов
* Добавлены необязательные `CancellationToken`-ы в методы взаимодействия с `ES`

## [1.2.11] - 2020-12-09

### Добавлено

* Методы расширения для логирования `IApiCallDetails`
* Добавление данных для логирования запроса в исключение `ResponseException<TResp>`
* Метод расширения для быстрого выкидывания исключения, если ответ от `ES` ошибочный
* Объектная модель поискового движка

### Изменено

* Сделан большой, ломающий обратную совместимость, рефакторинг 

## [1.1.0] - 2020-05-23

### Добавлено

* ES менеджер для конкретного индекса

## [1.0.0] - 2020-05-21

### Добавлено

* Конфигурирование подключения;
* Менеджер работы с ES (`IEsManager`);
* Методы расширения для индексации и поиска для `IEsManager`.