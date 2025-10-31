"""
Обработка данных из JSON файла
"""

import json
import sys

from .gen_random import gen_random
from .unique import Unique
from .print_result import print_result
from .cm_timer import cm_timer_1

# Получаем путь к файлу из аргументов командной строки
path = sys.argv[1] if len(sys.argv) > 1 else "data_light.json"

with open(path, encoding="utf-8") as f:
    data = json.load(f)


@print_result
def f1(arg):
    # Сортированный список профессий без повторений (игнорируя регистр)
    return sorted(
        list(Unique(map(lambda x: x["job-name"], arg), ignore_case=True)),
        key=lambda x: x.lower(),
    )


@print_result
def f2(arg):
    # Фильтрация элементов, начинающихся со слова "программист"
    return list(filter(lambda x: x.lower().startswith("программист"), arg))


@print_result
def f3(arg):
    # Добавление строки "с опытом Python"
    return list(map(lambda x: f"{x} с опытом Python", arg))


@print_result
def f4(arg):
    # Генерация зарплат и объединение с профессиями
    salaries = list(gen_random(len(arg), 100000, 200000))
    return [f"{job}, зарплата {salary} руб." for job, salary in zip(arg, salaries)]


if __name__ == "__main__":
    with cm_timer_1():
        f4(f3(f2(f1(data))))
