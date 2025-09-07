"""
Решение биквадратного уравнения (ООП вариант)
"""

from .untils import create_biquadratic_equation_string, solve_biquadratic, get_coef, parse_args, print_roots


class BiquadraticEquation:
    """
    Квадратное уравнение вида ax^2 + bx + c = 0
    """

    def __init__(self, a: float, b: float, c: float):
        self.a: float = a
        self.b: float = b
        self.c: float = c

    def solve(self) -> list[float] | list[str]:
        return solve_biquadratic(self.a, self.b, self.c)

    def __str__(self):
        return create_biquadratic_equation_string(self.a, self.b, self.c)


class EquationSolverApp:
    """
    Приложение для решения квадратных уравнений
    """

    def __init__(self):
        self.args = self.parse_args()

    def parse_args(self):
        return parse_args()

    def get_coef(self, prompt: str) -> float:
        return get_coef(prompt)

    def run(self):
        a = self.args.a if self.args.a is not None else self.get_coef("Введите A: ")
        b = self.args.b if self.args.b is not None else self.get_coef("Введите B: ")
        c = self.args.c if self.args.c is not None else self.get_coef("Введите C: ")

        eq = BiquadraticEquation(a, b, c)
        print(f"\nУравнение: {eq}")
        roots = eq.solve()

        print_roots(roots)


if __name__ == "__main__":
    EquationSolverApp().run()
