# SlidFinance
Многопользовательская система учета личных финансов, которая в автоматическом или ручном режиме импортирует информацию из банковских систем (10+). Помогает категорировать операции на основе имеющихся данных и выводит статистику. Ведет аналитику и строит финансовый отчет.

Данный репозиторий содержит серверную часть системы. Проект состоит из следующих компонентов:
1. **Backend и telegram bot**
2. [Frontend](https://github.com/SlidEnergy/slidfinance-frontend)
3. [Расширения Chrome для ручного импорта](https://github.com/SlidEnergy/slidfinance-plugins)

# Ручной импорт банковских операций
Ручной импорт осуществляется через расширения Chrome.

Поддерживаемые банки для ручного импорта:
1. Home Credit банк
2. МТС банк
3. Банк ФК Открытие
4. Банк ОТП Директ
5. Qiwi
6. Росгосстрах банк
7. Сбербанк
8. Скб банк
9. Тинькоф банк
10. Восточный банк
11. Яндекс деньги
12. Мегафон банк

# Автоматический импорт банковских операций
Автоматический импорт поддерживается через сервис [Saltedge](https://www.saltedge.com/). Есть провайдеры для более 3000 банков в более 60 стран.