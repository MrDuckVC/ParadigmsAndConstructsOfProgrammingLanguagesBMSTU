"""
Генератор случайных чисел в заданном диапазоне
"""

import random
from typing import Generator


def gen_random(num_count: int, begin: int, end: int) -> Generator[int, None, None]:
    """
    Генератор случайных чисел в заданном диапазоне

    Args:
        num_count (int): количество чисел
        begin (int): начало диапазона
        end (int): конец диапазона

    Yields:
        случайные числа
    """
    for _ in range(num_count):
        yield random.randint(begin, end)


if __name__ == "__main__":
    # Тестирование
    print("Тест gen_random:")
    for num in gen_random(5, 1, 3):
        print(num, end=" ")
    print()
