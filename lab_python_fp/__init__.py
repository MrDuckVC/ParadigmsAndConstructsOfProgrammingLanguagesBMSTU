"""
Пакет lab_python_fp - Функциональные возможности языка Python
Лабораторная работа №3-4
"""

from .field import field
from .gen_random import gen_random
from .unique import Unique
from .print_result import print_result
from .cm_timer import cm_timer_1, cm_timer_2

__all__ = ["field", "gen_random", "Unique", "print_result", "cm_timer_1", "cm_timer_2"]

__version__ = "1.0.0"
__author__ = "Valentin Cunev"
