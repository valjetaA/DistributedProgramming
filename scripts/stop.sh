process_names=("Valuator" "nginx")

for process_name in "${process_names[@]}"; do
    pids=$(pgrep "$process_name")

    for pid in $pids; do
        kill -9 "$pid"
    done

done
