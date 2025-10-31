"""
Пакет для лабораторной работы №2
Объектно-ориентированные возможности Python
"""

from .oop_biquadratic import EquationSolverApp
from .procedural_biquadratic import main as procedural_main

__all__ = ["EquationSolverApp", "procedural_main"]
