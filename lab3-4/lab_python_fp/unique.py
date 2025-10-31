"""
Итератор для удаления дубликатов
"""

from typing import Generator


class Unique(object):
    """
    Итератор для удаления дубликатов
    """

    def __init__(self, items: list | Generator, **kwargs):
        """
        Конструктор итератора

        Args:
            items (list | Generator): массив или генератор
            **kwargs: ignore_case - игнорировать регистр для строк
        """
        self.items = iter(items)
        self.ignore_case: bool = kwargs.get("ignore_case", False)
        self.seen = set()

    def __next__(self):
        while True:
            next_item = next(self.items)

            # Обработка ignore_case для строк
            if self.ignore_case and isinstance(next_item, str):
                key = next_item.lower()
            else:
                key = next_item

            if key not in self.seen:
                self.seen.add(key)
                return next_item

    def __iter__(self):
        return self


if __name__ == "__main__":
    # Тестирование
    from gen_random import gen_random

    print("Тест Unique с числами:")
    data1 = [1, 1, 1, 1, 1, 2, 2, 2, 2, 2]
    for item in Unique(data1):
        print(item, end=" ")
    print()

    print("Тест Unique со строками (ignore_case=False):")
    data2 = ["a", "A", "b", "B", "a", "A", "b", "B"]
    for item in Unique(data2):
        print(item, end=" ")
    print()

    print("Тест Unique со строками (ignore_case=True):")
    for item in Unique(data2, ignore_case=True):
        print(item, end=" ")
    print()

    print("Тест Unique с генератором:")
    data3 = gen_random(10, 1, 3)
    for item in Unique(data3):
        print(item, end=" ")
    print()
