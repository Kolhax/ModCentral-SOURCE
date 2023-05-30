using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CollectiveMinds.AppUpdate.Models;
using Semver;

namespace CollectiveMinds.AppUpdate;

public sealed class ApplicationUpdateManager
{
	public readonly SemVersion CurrentApplicationVersion;

	public readonly ApplicationUpdateManifest Manifest;

	public readonly bool UpdateAvailable;

	public readonly bool UpdateRequired;

	public static readonly bool ApplicationWasRelaunched;

	public ApplicationUpdateManager(ApplicationUpdateManifest manifest, ICurrentApplicationVersionSource currentVersionSource)
	{
		if (manifest == null)
		{
			throw new ArgumentNullException("manifest");
		}
		if (currentVersionSource == null)
		{
			throw new ArgumentNullException("currentVersionSource");
		}
		Manifest = manifest;
		CurrentApplicationVersion = currentVersionSource.GetCurrentApplicationVersion();
		UpdateAvailable = CurrentApplicationVersion < Manifest.LatestAvailableApplicationVersion;
		UpdateRequired = Manifest.MinimumRequiredApplicationVersion != null && CurrentApplicationVersion < Manifest.MinimumRequiredApplicationVersion;
	}

	static ApplicationUpdateManager()
	{
		ApplicationWasRelaunched = Environment.GetEnvironmentVariable("CM_UPDATE_RELAUNCHED") != null;
		Environment.SetEnvironmentVariable("CM_UPDATE_RELAUNCHED", null);
	}

	public static async Task PerformUpdate(byte[] updateData)
	{
		string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		string currentExecutablePath = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
		string backupExecutablePath = Path.Combine(tempDirectory, Path.GetFileName(currentExecutablePath));
		await Task.Run(() => Directory.CreateDirectory(tempDirectory)).ConfigureAwait(continueOnCapturedContext: false);
		try
		{
			await Task.Run(delegate
			{
				File.Move(currentExecutablePath, backupExecutablePath);
			}).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			await Task.Run(delegate
			{
				Directory.Delete(tempDirectory, recursive: true);
			}).ConfigureAwait(continueOnCapturedContext: false);
			throw;
		}
		try
		{
			await Task.Run(delegate
			{
				File.WriteAllBytes(currentExecutablePath, updateData);
			}).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			await Task.Run(delegate
			{
				File.Delete(currentExecutablePath);
				File.Move(backupExecutablePath, currentExecutablePath);
				Directory.Delete(tempDirectory, recursive: true);
			}).ConfigureAwait(continueOnCapturedContext: false);
			throw;
		}
		try
		{
			Process.Start(new ProcessStartInfo(currentExecutablePath)
			{
				EnvironmentVariables = 
				{
					{ "CM_UPDATE_RELAUNCHED", "1" },
					{ "CM_UPDATE_TEMP_DIR", tempDirectory }
				},
				UseShellExecute = false
			});
		}
		catch
		{
			await Task.Run(delegate
			{
				File.Delete(currentExecutablePath);
				File.Move(backupExecutablePath, currentExecutablePath);
				Directory.Delete(tempDirectory, recursive: true);
			});
			throw;
		}
	}

	public static void FinalizeUpdateAfterRelaunch()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("CM_UPDATE_TEMP_DIR");
		if (!string.IsNullOrEmpty(environmentVariable) && Directory.Exists(environmentVariable))
		{
			Directory.Delete(environmentVariable, recursive: true);
		}
		Environment.SetEnvironmentVariable("CM_UPDATE_TEMP_DIR", null);
	}

	public void VerifyUpdateData(byte[] updateData)
	{
		using HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
		string text = (Manifest as ApplicationUpdateManifestV2)?.LatestAvailableApplicationSha256Hash;
		if (string.IsNullOrWhiteSpace(text))
		{
			throw new InvalidOperationException("Manifest does not contain a hash to verify");
		}
		if (!string.Equals(hashAlgorithm.ComputeHash(updateData).ToHexString(), text, StringComparison.OrdinalIgnoreCase))
		{
			throw new VerificationException("Failed to verify hash, expected: {expected} got: {actual}");
		}
	}
}
