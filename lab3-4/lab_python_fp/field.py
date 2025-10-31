"""
Генератор для последовательного выдачи значений ключей словаря
"""

from typing import Generator


def field(items: list[dict], *args) -> Generator[any, None, None]:
    """
    Генератор для последовательного выдачи значений ключей словаря

    Args:
        items: список словарей
        *args: ключи для извлечения

    Yields:
        значения или словари в зависимости от количества аргументов
    """
    assert len(args) > 0, "Необходимо указать хотя бы один аргумент"

    for dict_item in items:
        if len(args) == 1:
            # Если один аргумент - возвращаем значение
            key = args[0]
            if key in dict_item and dict_item[key] is not None:
                yield dict_item[key]
        else:
            # Если несколько аргументов - возвращаем словарь
            result = {}
            has_values = False
            for key in args:
                if key in dict_item and dict_item[key] is not None:
                    result[key] = dict_item[key]
                    has_values = True

            if has_values:
                yield result


if __name__ == "__main__":
    # Тестирование
    goods = [
        {"title": "Ковер", "price": 2000, "color": "green"},
        {"title": "Диван для отдыха", "color": "black"},
        {"title": None, "price": 5000},
        {"price": 3000, "color": "white"},
    ]

    print("Тест field с одним аргументом:")
    for item in field(goods, "title"):
        print(item)

    print("\nТест field с несколькими аргументами:")
    for item in field(goods, "title", "price"):
        print(item)
