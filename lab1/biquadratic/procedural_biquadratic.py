"""
Решение биквадратного уравнения (процедурный вариант)
"""

from .untils import create_biquadratic_equation_string, parse_args, get_coef, print_roots, solve_biquadratic


def main():
    print("=== Процедурный вариант решения биквадратного уравнения ===")
    args = parse_args()
    a = args.a if args.a is not None else get_coef("Введите A: ")
    b = args.b if args.b is not None else get_coef("Введите B: ")
    c = args.c if args.c is not None else get_coef("Введите C: ")

    print(f"\nУравнение: {create_biquadratic_equation_string(a, b, c)}")
    roots = solve_biquadratic(a, b, c)
    print_roots(roots)


if __name__ == "__main__":
    main()
