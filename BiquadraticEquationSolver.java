import java.util.*;
import java.util.stream.Collectors;

public class BiquadraticEquationSolver {

    private static final String INFINITY_ROOTS_TEXT = "Бесконечное множество решений";

    public static void main(String[] args) {
        System.out.println("=== Процедурный вариант решения биквадратного уравнения ===");

        double a, b, c;

        if (args.length >= 3) {
            a = Double.parseDouble(args[0]);
            b = Double.parseDouble(args[1]);
            c = Double.parseDouble(args[2]);
        } else {
            a = getCoefficient("Введите A: ");
            b = getCoefficient("Введите B: ");
            c = getCoefficient("Введите C: ");
        }

        System.out.println("\nУравнение: " + createBiquadraticEquationString(a, b, c));
        List<Object> roots = solveBiquadratic(a, b, c);
        printRoots(roots);
    }

    /**
     * Создание строки для квадратного уравнения
     */
    public static String createBiquadraticEquationString(double a, double b, double c) {
        String aStr = formatNumber(a);
        String bStr = formatNumber(Math.abs(b));
        String cStr = formatNumber(Math.abs(c));

        String bSign = b >= 0 ? "+" : "-";
        String cSign = c >= 0 ? "+" : "-";

        return aStr + "x^4 " + bSign + " " + bStr + "x^2 " + cSign + " " + cStr + " = 0";
    }

    /**
     * Форматирование числа (целое или дробное)
     */
    private static String formatNumber(double x) {
        if (x == (int) x) {
            return Integer.toString((int) x);
        }
        return Double.toString(x);
    }

    /**
     * Получение коэффициента с клавиатуры
     */
    public static double getCoefficient(String prompt) {
        Scanner scanner = new Scanner(System.in);
        while (true) {
            try {
                System.out.print(prompt);
                return scanner.nextDouble();
            } catch (InputMismatchException e) {
                System.out.println("Некорректный ввод. Попробуйте снова.");
                scanner.next(); // очистить буфер
            }
        }
    }

    /**
     * Вывод корней
     */
    public static void printRoots(List<Object> roots) {
        if (roots.isEmpty()) {
            System.out.println("Действительных корней нет");
        } else if (roots.size() == 1 && roots.get(0) instanceof String) {
            System.out.println(roots.get(0));
        } else {
            System.out.println("Найдено " + roots.size() + " действительных корней:");
            for (int i = 0; i < roots.size(); i++) {
                System.out.printf("  x%d = %.4f%n", i + 1, (Double) roots.get(i));
            }
        }
    }

    /**
     * Решение биквадратного уравнения
     */
    public static List<Object> solveBiquadratic(double a, double b, double c) {
        if (a == 0 && b == 0 && c == 0) {
            return Arrays.asList(INFINITY_ROOTS_TEXT);
        }

        if (a == 0) {
            if (b == 0) {
                return new ArrayList<>();
            }
            double y = -c / b;
            if (y < 0) {
                return new ArrayList<>();
            }
            List<Object> roots = new ArrayList<>();
            roots.add(Math.sqrt(y));
            roots.add(-Math.sqrt(y));
            return sortRoots(roots);
        }

        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0) {
            return new ArrayList<>();
        }

        List<Object> roots = new ArrayList<>();
        double y1 = (-b + Math.sqrt(discriminant)) / (2 * a);
        double y2 = (-b - Math.sqrt(discriminant)) / (2 * a);

        if (y1 >= 0) {
            roots.add(Math.sqrt(y1));
            roots.add(-Math.sqrt(y1));
        }

        if (y2 >= 0) {
            roots.add(Math.sqrt(y2));
            roots.add(-Math.sqrt(y2));
        }

        return sortRoots(removeDuplicates(roots));
    }

    /**
     * Удаление дубликатов корней
     */
    private static List<Object> removeDuplicates(List<Object> roots) {
        Set<Object> uniqueRoots = new LinkedHashSet<>();
        for (Object root : roots) {
            if (root instanceof Double) {
                // Округляем для сравнения (из-за погрешности вычислений с плавающей точкой)
                double rounded = Math.round((Double) root * 1e10) / 1e10;
                uniqueRoots.add(rounded);
            } else {
                uniqueRoots.add(root);
            }
        }
        return new ArrayList<>(uniqueRoots);
    }

    /**
     * Сортировка корней
     */
    private static List<Object> sortRoots(List<Object> roots) {
        List<Double> numericRoots = new ArrayList<>();
        List<Object> otherRoots = new ArrayList<>();

        for (Object root : roots) {
            if (root instanceof Double) {
                numericRoots.add((Double) root);
            } else {
                otherRoots.add(root);
            }
        }

        Collections.sort(numericRoots);
        List<Object> result = new ArrayList<>();
        result.addAll(otherRoots);
        result.addAll(numericRoots);

        return result;
    }
}
