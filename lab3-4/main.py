#!/usr/bin/env python3
"""
Основной файл для тестирования всех модулей
"""

import sys
import os
from time import sleep

from lab_python_fp import field, gen_random, Unique, cm_timer_1, cm_timer_2
from lab_python_fp.sort import data as data_sort
from lab_python_fp.print_result import test_1, test_2, test_3, test_4

# Добавляем путь к пакету
sys.path.insert(0, os.path.join(os.path.dirname(__file__)))


def test_all_modules():
    """Тестирование всех модулей"""
    print("=== Тестирование всех модулей ===\n")

    # Тестирование field
    print("1. Тестирование field:")

    goods = [
        {"title": "Ковер", "price": 2000, "color": "green"},
        {"title": "Диван для отдыха", "color": "black"},
    ]
    print("field(goods, 'title'):", list(field(goods, "title")))
    print("field(goods, 'title', 'price'):", list(field(goods, "title", "price")))
    print()

    # Тестирование gen_random
    print("2. Тестирование gen_random:")

    print("gen_random(5, 1, 3):", list(gen_random(5, 1, 3)))
    print()

    # Тестирование unique
    print("3. Тестирование unique:")

    data = [1, 1, 2, 2, 3, 3]
    print(f"Unique({data}):", list(Unique(data)))
    print()

    # Тестирование sort
    print("4. Тестирование sort:")

    result = sorted(data_sort, key=abs, reverse=True)
    print("Исходные данные:", data_sort)
    print("Отсортировано по модулю:", result)
    print()

    # Тестирование print_result
    print("5. Тестирование print_result:")

    test_1()
    test_2()
    test_3()
    test_4()
    print()

    # Тестирование cm_timer
    print("6. Тестирование cm_timer:")

    print("cm_timer_1 (1 секунда):")
    with cm_timer_1():
        sleep(1)

    print("cm_timer_2 (0.5 секунды):")
    with cm_timer_2():
        sleep(0.5)
    print()


if __name__ == "__main__":
    test_all_modules()
