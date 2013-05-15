using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Generate.Program;
using System.IO;
using Encog.ML;
using Encog.Persist;
using Encog.ML.Data;
using Encog.Util.Simple;
using Encog.Util.CSV;

namespace Encog.App.Generate.Generators.Java
{
    /// <summary>
    /// Generate Java.
    /// </summary>
    public class GenerateEncogJava : AbstractGenerator
    {
        private bool embed;

	private void EmbedNetwork(EncogProgramNode node) {
		AddBreak();

		FileInfo methodFile = (FileInfo) node.Args[0].Value;

		IMLMethod method = (IMLMethod) EncogDirectoryPersistence
				.LoadObject(methodFile);

		if (!(method is IMLFactory)) {
			throw new EncogError("Code generation not yet supported for: "
					+ typeof(method).Name);
		}

		IMLFactory factoryMethod = (IMLFactory) method;

		String methodName = factoryMethod.FactoryType;
		String methodArchitecture = factoryMethod.FactoryArchitecture;

		// header
		AddInclude("org.encog.ml.MLMethod");
		AddInclude("org.encog.persist.EncogDirectoryPersistence");

		StringBuilder line = new StringBuilder();
		line.Append("public static MLMethod ");
		line.Append(node.Name);
		line.Append("() {");
		indentLine(line.ToString());

		// create factory
		line.Length = 0;
		AddInclude("org.encog.ml.factory.MLMethodFactory");
		line.Append("MLMethodFactory methodFactory = new MLMethodFactory();");
		AddLine(line.ToString());

		// factory create
		line.Length = 0;
		line.Append("MLMethod result = ");

		line.Append("methodFactory.create(");
		line.Append("\"");
		line.Append(methodName);
		line.Append("\"");
		line.Append(",");
		line.Append("\"");
		line.Append(methodArchitecture);
		line.Append("\"");
		line.Append(", 0, 0);");
		AddLine(line.ToString());

		line.Length = 0;
		AddInclude("org.encog.ml.MLEncodable");
		line.Append("((MLEncodable)result).decodeFromArray(WEIGHTS);");
		AddLine(line.ToString());

		// return
		AddLine("return result;");

		UnIndentLine("}");
	}

	private void EmbedTraining(EncogProgramNode node) {

		FileInfo dataFile = (FileInfo) node.Args[0].Value;
		IMLDataSet data = EncogUtility.LoadEGB2Memory(dataFile);

		// generate the input data

		IndentLine("public static final double[][] INPUT_DATA = {");
		foreach (IMLDataPair pair in data) {
			IMLData item = pair.Input;

			StringBuilder line = new StringBuilder();

			NumberList.ToList(CSVFormat.EgFormat, line, item.Data );
			line.Insert(0, "{ ");
			line.Append(" },");
			AddLine(line.ToString());
		}
		UnIndentLine("};");

		AddBreak();

		// generate the ideal data

		IndentLine("public static final double[][] IDEAL_DATA = {");
		foreach (IMLDataPair pair in data) {
			IMLData item = pair.Ideal;

			StringBuilder line = new StringBuilder();

			NumberList.ToList(CSVFormat.EgFormat, line, item.Data);
			line.Insert(0, "{ ");
			line.Append(" },");
			AddLine(line.ToString());
		}
		UnIndentLine("};");
	}

	public void Generate(EncogGenProgram program, bool shouldEmbed) {
		this.embed = shouldEmbed;
		GenerateForChildren(program);
		GenerateImports(program);
	}

	private void generateArrayInit(final EncogProgramNode node) {
		final StringBuilder line = new StringBuilder();
		line.append("public static final double[] ");
		line.append(node.getName());
		line.append(" = {");
		indentLine(line.toString());

		final double[] a = (double[]) node.getArgs().get(0).getValue();

		line.setLength(0);

		int lineCount = 0;
		for (int i = 0; i < a.length; i++) {
			line.append(CSVFormat.EG_FORMAT.format(a[i],
					Encog.DEFAULT_PRECISION));
			if (i < (a.length - 1)) {
				line.append(",");
			}

			lineCount++;
			if (lineCount >= 10) {
				addLine(line.toString());
				line.setLength(0);
				lineCount = 0;
			}
		}

		if (line.length() > 0) {
			addLine(line.toString());
			line.setLength(0);
		}

		unIndentLine("};");
	}

	private void generateClass(final EncogProgramNode node) {
		addBreak();
		indentLine("public class " + node.getName() + " {");
		generateForChildren(node);
		unIndentLine("}");
	}

	private void generateComment(final EncogProgramNode commentNode) {
		addLine("// " + commentNode.getName());
	}

	private void generateConst(final EncogProgramNode node) {
		final StringBuilder line = new StringBuilder();
		line.append("public static final ");
		line.append(node.getArgs().get(1).getValue());
		line.append(" ");
		line.append(node.getName());
		line.append(" = \"");
		line.append(node.getArgs().get(0).getValue());
		line.append("\";");

		addLine(line.toString());
	}

	private void generateCreateNetwork(final EncogProgramNode node) {
		if (this.embed) {
			embedNetwork(node);
		} else {
			linkNetwork(node);
		}
	}

	private void generateEmbedTraining(final EncogProgramNode node) {
		if (this.embed) {
			embedTraining(node);
		}
	}

	private void generateForChildren(final EncogTreeNode parent) {
		for (final EncogProgramNode node : parent.getChildren()) {
			generateNode(node);
		}
	}

	private void generateFunction(final EncogProgramNode node) {
		addBreak();

		final StringBuilder line = new StringBuilder();
		line.append("public static void ");
		line.append(node.getName());
		line.append("() {");
		indentLine(line.toString());

		generateForChildren(node);
		unIndentLine("}");
	}

	private void generateFunctionCall(final EncogProgramNode node) {
		addBreak();
		final StringBuilder line = new StringBuilder();
		if (node.getArgs().get(0).getValue().toString().length() > 0) {
			line.append(node.getArgs().get(0).getValue().toString());
			line.append(" ");
			line.append(node.getArgs().get(1).getValue().toString());
			line.append(" = ");
		}

		line.append(node.getName());
		line.append("();");
		addLine(line.toString());
	}

	private void generateImports(final EncogGenProgram program) {
		final StringBuilder imports = new StringBuilder();
		for (final String str : getIncludes()) {
			imports.append("import ");
			imports.append(str);
			imports.append(";\n");
		}

		imports.append("\n");

		addToBeginning(imports.toString());

	}

	private void generateLoadTraining(final EncogProgramNode node) {
		addBreak();

		final File methodFile = (File) node.getArgs().get(0).getValue();

		addInclude("org.encog.ml.data.MLDataSet");
		final StringBuilder line = new StringBuilder();
		line.append("public static MLDataSet createTraining() {");
		indentLine(line.toString());

		line.setLength(0);

		if (this.embed) {
			addInclude("org.encog.ml.data.basic.BasicMLDataSet");
			line.append("MLDataSet result = new BasicMLDataSet(INPUT_DATA,IDEAL_DATA);");
		} else {
			addInclude("org.encog.util.simple.EncogUtility");
			line.append("MLDataSet result = EncogUtility.loadEGB2Memory(new File(\"");
			line.append(methodFile.getAbsolutePath());
			line.append("\"));");
		}

		addLine(line.toString());

		// return
		addLine("return result;");

		unIndentLine("}");
	}

	private void generateMainFunction(final EncogProgramNode node) {
		addBreak();
		indentLine("public static void main(String[] args) {");
		generateForChildren(node);
		unIndentLine("}");
	}

	private void generateNode(final EncogProgramNode node) {
		switch (node.getType()) {
		case Comment:
			generateComment(node);
			break;
		case Class:
			generateClass(node);
			break;
		case MainFunction:
			generateMainFunction(node);
			break;
		case Const:
			generateConst(node);
			break;
		case StaticFunction:
			generateFunction(node);
			break;
		case FunctionCall:
			generateFunctionCall(node);
			break;
		case CreateNetwork:
			generateCreateNetwork(node);
			break;
		case InitArray:
			generateArrayInit(node);
			break;
		case EmbedTraining:
			generateEmbedTraining(node);
			break;
		case LoadTraining:
			generateLoadTraining(node);
			break;
		}
	}

	private void linkNetwork(final EncogProgramNode node) {
		addBreak();

		final File methodFile = (File) node.getArgs().get(0).getValue();

		addInclude("org.encog.ml.MLMethod");
		final StringBuilder line = new StringBuilder();
		line.append("public static MLMethod ");
		line.append(node.getName());
		line.append("() {");
		indentLine(line.toString());

		line.setLength(0);
		line.append("MLMethod result = (MLMethod)EncogDirectoryPersistence.loadObject(new File(\"");
		line.append(methodFile.getAbsolutePath());
		line.append("\"));");
		addLine(line.toString());

		// return
		addLine("return result;");

		unIndentLine("}");
	}
    }
}
